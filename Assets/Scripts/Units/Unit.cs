using System;
using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public const float AnimationWindow = 0.1f;
    public int HP = 1;
    [NonSerialized]
    public bool JustPopped = false;

    private void Start()
    {
        _actPos = transform.position;
        _actScale = transform.localScale;
        Level.Instance.InitPos(this);
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
            return true;
        }

        Move(relDir);
        return true;
    }

    public void Move(int relDir)
    {
        Level.Instance.Move(Position.IntX() + relDir, this);
        Position += new Vector3(relDir, 0, 0);
    }

    public virtual void TakeDmg(Unit source, int dmg = 1)
    {
        Debug.Log("take dmg " + this.name + " " + source.name);
        HP -= dmg;
        HitEffect.Create(transform.position, this);
        if (HP <= 0)
        {
            Die();
        }
        else
        {
            var dir = Position.IntX() - source.Position.IntX();
            dir = dir > 0 ? 1 : -1;
            Move(dir);
        }
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

    public virtual void Die()
    {
        Level.Instance.Clear(Position.IntX());
        Destroy(gameObject);
        
        if (Level.Instance.EnemiesCount == 0 && !(this is Player))
        {
            Level.TickTime /= 1.5f;
            CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.43f, 0.43f, 0.43f), 2f, true);
            Level.Instance.Restart();
            CounterScript.Instance.IncreaseCounter();
        }
    }
}