using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{
    public const float AnimationWindow = 0.05f;
    public int HP = 1;
    [NonSerialized]
    public bool JustPopped = false;
    private static readonly Prefab ShieldPrefab = new Prefab("Shield");
    protected GameObject Shield;
    public int RunningAnimations = 0;
    protected Material HpMat;

    public bool Shielded;

    private void Start()
    {
        _actPos = transform.position;
        _actScale = transform.localScale;
        Level.Instance.InitPos(this);
        HpMat = GetComponent<SpriteRenderer>().material;
        HpMat.SetFloat("_Rot", Random.Range(-1f, 1f));
        HpMat.SetFloat("_Center", Random.Range(0.35f, 0.55f));
        HpMat.SetInt("_Hp", HP);
        HpMat.SetInt("_CurHp", HP);
        
        if (Shielded)
        {
            CreateShield();
        }
    }

    protected void CreateShield()
    {
        Shield = ShieldPrefab.Instantiate();
        Shield.transform.SetParent(transform);
        Shield.transform.localPosition = Vector3.zero;
    }

    protected virtual bool MoveOrAttack(int relDir)
    {
        var pos = relDir + Position.IntX();
        var atPos = Level.Instance.Get(pos);
        if (atPos != null)
        {
            bool class1 = atPos is Player, class2 = this is Player;
            if (class1 == class2) return false;
            atPos.TakeDmg(this);
            AttackAnim(relDir, atPos);
            if (class1)
            {
                Move(relDir);
            }
            return true;
        }

        Move(relDir);
        return true;
    }

    protected void AttackAnim(int relDir, Unit target)
    {
        var change = new Vector3(0.5f, -0.5f);
        Utils.Animate(Vector3.zero, change, 0.001f, v => transform.localScale += v,
            this);
        Utils.Animate(change, Vector3.zero, AnimationWindow, v => transform.localScale += v,
            this);
        Utils.Animate(_actPos, _actPos + new Vector3((float) relDir / 1.5f, 0, 0), AnimationWindow,
            v => transform.position += v, this);
        change = -change;
        target.TakeDmgAnim(AnimationWindow);
        Utils.Animate(_actPos + new Vector3((float) relDir / 1.5f, 0, 0), _actPos, AnimationWindow,
            v => transform.position += v,
            this, false, AnimationWindow);
        Utils.Animate(Vector3.zero, change, 0.001f, v => transform.localScale += v,
            this, false, AnimationWindow);
        Utils.Animate(change, Vector3.zero, AnimationWindow, v => transform.localScale += v,
            this, false, AnimationWindow);
    }

    public void Move(int relDir)
    {
        var change = new Vector3(0.5f, -0.5f);
        Utils.Animate(Vector3.zero, change, 0.001f, v => transform.localScale += v,
            this);
        Utils.Animate(change, Vector3.zero, AnimationWindow * 2, v => transform.localScale += v,
            this);
        Level.Instance.Move(Position.IntX() + relDir, this);
        Position += new Vector3(relDir, 0, 0);
    }

    public void TakeDmgAnim(float delay)
    {
        Invoke("_TakeDmgAnim", delay);
    }

    private void _TakeDmgAnim()
    {
        if (HadShield)
        {
            HadShield = false;
            ShieldDieEffect.Create(transform.position);
        }
        else
        {
            HitEffect.Create(transform.position, this);
            if (HP <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    protected bool HadShield = false;
    public virtual void TakeDmg(Unit source, int dmg = 1)
    {
        if (Shield != null)
        {
//            var shieldMat = Shield.GetComponent<SpriteRenderer>().material;
//            Utils.Animate(6f, 1f, Level.TickTime * 2, (v) => shieldMat.SetFloat("_Percentage", v), this, true);
            HadShield = true;
            DestroyShield();
//            var dir = Position.IntX() - source.Position.IntX();
//            dir = dir > 0 ? 1 : -1;
//            Move(dir);
            return;
        }
        HP -= dmg;
        HpMat.SetInt("_CurHp", HP);
        if (HP <= 0)
        {
            Die();
        }
    }

    protected void DestroyShield()
    {
        Destroy(Shield);
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

    private Vector3 _actPos;
    public Vector3 Position
    {
        get { return _actPos; }
        set
        {
            Utils.Animate(_actPos, value, AnimationWindow, v =>
            {
                transform.position += v;
                if (this is Player && 2 - Math.Abs(transform.position.x) < 0.001)
                {
                    TakeDmg(this, 9999);
                    TakeDmgAnim(0);
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
            Utils.Animate(_actScale, value, AnimationWindow, v => transform.localScale += v, this);
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
        
        if (Level.Instance.EnemiesCount == 0 && !(this is Player))
        {
//            CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.43f, 0.43f, 0.43f), 1.5f, true);
            
        }
    }
}