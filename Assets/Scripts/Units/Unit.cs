﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{
    public const float AnimationWindow = 0.1f;
    public float AnimationSpeedUp = 1f;
    public int HP = 1;
    public int MaxHP;
    [NonSerialized]
    public bool JustPopped = false;
    public int RunningAnimations = 0;
    protected Material HpMat;
    public Action DieEvent = () => { };

    private void Start()
    {
        MaxHP = HP;
        _actPos = transform.position;
        _actScale = transform.localScale;
        if (Level.Instance != null)
        {
            Level.Instance.InitPos(this);
        }
        HpMat = GetComponent<SpriteRenderer>().material;
        HpMat.SetFloat("_Rot", Random.Range(-1f, 1f));
        HpMat.SetFloat("_Center", Random.Range(0.35f, 0.55f));
        HpMat.SetInt("_Hp", HP);
        HpMat.SetInt("_CurHp", HP);
        
        SpawnEffect.Create(transform.position, this);
    }

    protected virtual bool MoveOrAttack(int relDir)
    {
        if (HP <= 0)
        {
            return true;
        }
        if (Player.Instance.HP <= 0)
        {
            return true;
        }
        var pos = relDir + Position.IntX();
        
        if (Math.Abs(pos) > Level.Size / 2)
        {
            return true;
        }
        var atPos = Level.Instance.Get(pos);
//        if (Level.Updating && this is Player && atPos == null)
//        {
//            var newST = 1.0f * Level.Instance.AudioSource.timeSamples / 44100 + Level.Instance.BeatOffset;
//            var bt = Level.TickTime * 3;
//            if (newST - Math.Floor(newST / bt) * bt > bt / 4 * 3)
//            {
//                var nextPos = relDir * 2 + Position.IntX();
//                var atNextPos = Level.Instance.Get(nextPos);
//                if (atNextPos != null)
//                {
//                    Debug.LogWarning("Early hit");
//                    (atNextPos as BasicEnemy).SkipTurn = true;
//                }
//            }
//            
//        }
        if (atPos != null)
        {
            bool class1 = atPos is Player, class2 = this is Player;
            if (class1 == class2) return false;
            if (atPos is DownTriangleEnemy)
            {
                Move(relDir);
                atPos.Move(relDir);
                return true;
            }
            if (atPos is CircleEnemy && !Player.Godmode)
            {
                (atPos as CircleEnemy).Swallow();
                return true;
            }
            if (class1)
            {
                if (this is DownTriangleEnemy)
                {
                    Move(relDir, true);
                    Level.Instance.Clear(Position.IntX());
                    Utils.InvokeDelayed(() =>
                    {
                        HitEffect.Create(transform.position, this);
                        Destroy(gameObject);
                    }, AnimationWindow / 2, this);
                    DieEvent();
                    return true;
                }
                if (this is CircleEnemy && Player.Instance.Swallowed == null)
                {
                    (this as CircleEnemy).Swallow();
                    return false;
                }
                if (Player.Instance.PushTicks == Level.Ticks)
                {
                    return true;
                }
                var nextPos = relDir * 2 + Position.IntX();
                var atNextPos = Level.Instance.Get(nextPos);
                if (atNextPos == null)
                {
                    var gm = GridMarks.Instance;
                    if (Player.Instance.Swallowed == null && (!gm.LeftSolid || !(gm.RightBorder.transform.position.x < nextPos) &&
                        !(gm.LeftBorder.transform.position.x > nextPos)))
                    {
                        Player.Instance.Move(relDir, false, true);
                        Player.Instance.PushTicks = Level.Ticks;
                        Move(relDir);
//                    AttackAnim(relDir);
                        return true;
                    }
                }
                if (Level.JustFinishedTutorial && Level.GameOver)
                {
                    return true;
                }
                if (atNextPos != null) atNextPos.AttackAnim(-relDir);
                Time.timeScale = 0.1f;
                Utils.Animate(0.1f, 1f, 0.5f, (v) => Time.timeScale = v, null, true);
                var camSize = Camera.main.orthographicSize;
                Utils.Animate(camSize, camSize / 1.5f, 0.07f, (v) => Camera.main.orthographicSize += v, null, false, 0f,
                    InterpolationType.InvSquare);
                Utils.InvokeDelayed(
                    () => Utils.Animate(Camera.main.orthographicSize, camSize, 0.2f,
                        (v) => Camera.main.orthographicSize += v), 0.5f);
            }
            if (Player.Instance.TutorialHitChanceDir != 0)
            {
                Player.Instance.TutorialHitChanceDir = 0;
                if (!(this is Player))
                {
                    Level.Instance.AudioSource.pitch = 1f;
                }
                else
                {
                    Utils.Animate(0.1f, 1f, 0.1f, (v) =>
                    {
                        Time.timeScale = v;
                        Level.Instance.AudioSource.pitch = v;
                    }, null, true);
                }
            }
            atPos.TakeDmg(this);
            AttackAnim(relDir);
            return true;
        }
        if (Level.Instance.Get(Position.IntX() + relDir * 2) is Player)
        {
            UpdateKiller(GetPrefab(), MaxHP);
        }

        Move(relDir);
        if (!Level.Tutorial && this is Player)
        {
            var atBehind = Level.Instance.Get(Position.IntX() - relDir * 2);
            if (atBehind != null)
            {
                atBehind.Move(relDir);
            }
        }
        return true;
    }

    private static Prefab[] killersPriorities = new []{
        BasicEnemy.Prefab,
        RhombusEnemy.Prefab,
        TriangleEnemy.Prefab,
        CircleEnemy.Prefab,
        DownTriangleEnemy.Prefab
    };

    private static int _updateTicks = 0;

    private static void UpdateKiller(Prefab p, int HP)
    {
        if (Level.Instance.Killer == null || (Level.Ticks - _updateTicks > 12 && (p != BasicEnemy.Prefab || HP > 1)))
        {
            Level.Instance.Killer = p;
            Level.Instance.KillerHP = HP;
//            Debug.LogWarning("Killer = " + p + " " + Level.Instance.KillerHP );
            _updateTicks = Level.Ticks;
            return;
        }

        var found = false;
        foreach (var prefab in killersPriorities)
        {
            if (Level.Instance.Killer == prefab)
            {
                found = true;
            }
            if (prefab != p) continue;
            if (!found) break;
            Level.Instance.KillerHP = Level.Instance.Killer == prefab ? Math.Max(HP, Level.Instance.KillerHP) : HP;
            _updateTicks = Level.Ticks;
            Level.Instance.Killer = p;
            return;
        }
    }

    protected void AttackAnim(int relDir)
    {
        var change = new Vector3(0.5f, -0.5f);
        const float awHalf = AnimationWindow / 2;
        Utils.Animate(Vector3.zero, change, 0.001f, v => transform.localScale += v,
            this);
        Utils.Animate(change, Vector3.zero, awHalf, v => transform.localScale += v,
            this);
        Utils.Animate(_actPos, _actPos + new Vector3(relDir / 1.5f, 0, 0), awHalf,
            v => transform.position += v, this);
        change = -change;
        Utils.Animate(_actPos + new Vector3(relDir / 1.5f, 0, 0), _actPos, awHalf,
            v => transform.position += v,
            this, false, awHalf);
        Utils.Animate(Vector3.zero, change, 0.001f, v => transform.localScale += v,
            this, false, awHalf);
        Utils.Animate(change, Vector3.zero, awHalf, v => transform.localScale += v,
            this, false, awHalf);
        var p = this as Player;
        if (p != null)
        {
            var vec = relDir > 0 ? new Vector3(0.5f, 0f) : new Vector3(-0.5f, 0f);
            Utils.Animate(Vector3.zero, vec, AnimationWindow / 2, (v) =>
            {
                p.Spikes.transform.localPosition += v;
                p.Spikes.transform.localScale += v * 5;
            }, this);
            Utils.Animate(vec, Vector3.zero, AnimationWindow / 2, (v) =>
            {
                p.Spikes.transform.localPosition += v;
                p.Spikes.transform.localScale += v * 5;
            }, this, false, AnimationWindow);
        }
    }

    public void Move(int relDir, bool onlyAnim = false, bool noSpikes = false)
    {
        if (this is Player)
        {
            var gm = GridMarks.Instance;
            if (gm.RightSolid && gm.RightBorder.transform.position.x < Position.x + relDir)
            {
                AttackAnim(relDir);
                GridMarks.Instance.HandlerRight();
                return;
            }
            if (gm.LeftSolid && gm.LeftBorder.transform.position.x > Position.x + relDir)
            {
                AttackAnim(relDir);
                GridMarks.Instance.HandlerLeft();
                return;
            }
        } else if (Level.Tutorial && Player.Instance.HasTutorialHitChance)
        {
            var near = Level.Instance.Get(Position.IntX() + relDir * 2);
            
            if (near is Player)
            {
                Player.Instance.HasTutorialHitChance = false;
                Player.Instance.TutorialHitChanceDir = -relDir;
                var str = Player.Instance.TutorialHitChanceDir == 1 ? ">" : "<";
                var hintGo = UnitHint.CreateUnitText(str, this, 0f, 0f, 2f, Color.white);
                Utils.Animate(0f, 1f, 1f, (v) =>
                {
                    if (hintGo == null) return;
                    var d = Math.Round(v / 0.05f) % 2 == 0;
                    var k = 0.1f;
                    hintGo.transform.position += Vector3.right * (d ? k : -k);
                }, hintGo, true);
                Time.timeScale = 0.1f;
                Utils.Animate(1f, 0.1f, 0.05f, (v) => Level.Instance.AudioSource.pitch = v, null, true);
            }
        }

        var change = new Vector3(0.5f, -0.5f);
        Utils.Animate(Vector3.zero, change, AnimationWindow / 2, v => transform.localScale += v,
            this);
        Utils.InvokeDelayed(() => Utils.Animate(change, Vector3.zero, AnimationWindow / 2,
            v => transform.localScale += v,
            this), AnimationWindow / 3 * 2, this);
        var p = this as Player;
        if (p != null && !noSpikes)
        {
            var vec = relDir > 0 ? new Vector3(0.5f, 0f) : new Vector3(-0.5f, 0f);
            Utils.Animate(Vector3.zero, vec, AnimationWindow / 2, (v) =>
            {
                p.Spikes.transform.localPosition += v;
                p.Spikes.transform.localScale += v * 5;
            }, this);
            Utils.Animate(vec, Vector3.zero, AnimationWindow / 2, (v) =>
            {
                p.Spikes.transform.localPosition += v;
                p.Spikes.transform.localScale += v * 5;
            }, this, false, AnimationWindow);
        }

        if (!onlyAnim) Level.Instance.Move(Position.IntX() + relDir, this);
        Position += new Vector3(relDir, 0, 0);
        if (onlyAnim) _actPos -= new Vector3(relDir, 0, 0);
    }

    private void _TakeDmgAnim()
    {
        HitEffect.Create(transform.position, this);
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual bool TakeDmg(Unit source, int dmg = 1)
    {
        Utils.InvokeDelayed(_TakeDmgAnim, AnimationWindow / 2, this);
        HP -= dmg;
        HpMat.SetInt("_CurHp", HP);
        if (HP <= 0)
        {
            if (Level.Tutorial && MaxHP > 1 && source is Player)
            {
                Level.Tutorial = false;
                PlayerData.Instance.TutorialComplete = true;
                Level.JustFinishedTutorial = true;
                Saves.Save();
            }
            Die();
            return true;
        }
        return false;
    }

    private GameObject _pe;
    public void SetPushedVisuals(bool v)
    {
        var sr = GetComponent<SpriteRenderer>();
        var c = sr.color;
        c.a = v ? 0.3f : 1f;
        sr.color = c;
        Scale = v ? new Vector3(0.7f, 0.7f, 1f) : Vector3.one;

        if (v)
        {
            if (_pe != null) return;
            _pe = PushedEffect.Create();
            _pe.transform.parent = transform;
            _pe.transform.position = transform.position;
        }
        else
        {
            Destroy(_pe);
        }
    }

    protected Vector3 _actPos;
    public Vector3 Position
    {
        get { return _actPos; }
        set
        {
            Utils.Animate(_actPos, value, AnimationWindow / AnimationSpeedUp, v =>
            {
                transform.position += v;
                var player = this as Player;
                if (player == null) return;
                var pos = player.transform.position.x;
                if (HP <= 0) return;
                if (!GridMarks.Instance.LeftSolid && pos < Math.Floor(GridMarks.Instance.LeftBorder.transform.position.x) + 0.01f)
                {
                    GridMarks.Instance.HandlerLeft();
                    Player.Instance.DieEvent = () => { };
                    Player.Instance.TakeDmg(Player.Instance, 999);
                }
                else if (!GridMarks.Instance.RightSolid && pos > Math.Ceiling(GridMarks.Instance.RightBorder.transform.position.x) - 0.01f)
                {
                    GridMarks.Instance.HandlerRight();
                    Player.Instance.DieEvent = () => { };
                    Player.Instance.TakeDmg(Player.Instance, 999);
                }
            }, this);

            _actPos = value;
        }
    }

    private Vector3 _actScale;

    public Vector3 Scale
    {
        get { return _actScale; }
        set
        {
            Utils.Animate(_actScale, value, AnimationWindow / AnimationSpeedUp, v => transform.localScale += v, this);
            _actScale = value;
        }
    }

    public virtual bool TickUpdate()
    {
        return true;
    }

    private int animCount = 3;
    public void TickAnimations()
    {
        var change = new Vector3(0f, 0.1f);
        if (animCount % 3 == 0)
        {
            Utils.Animate(Vector3.zero, change, 0.1f, v => transform.localScale += v,
                this);
            Utils.Animate(change, Vector3.zero, 0.3f, v => transform.localScale += v,
                this);
        }
        animCount++;
    }

    public virtual void Die()
    {
        Level.Instance.Clear(Position.IntX());
        DieEvent();
    }

    public virtual Prefab GetPrefab()
    {
        return null;
    } 
}