using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public const float AnimationWindow = 0.1f;
    public int HP = 1;
    public bool JustPopped = false;

    private void Start()
    {
        _tPos = transform.position;
        Level.Instance.InitPos(this);
        PlayAnimations();
    }

    protected bool MoveOrAttack(int relDir)
    {
        var pos = relDir + Position.IntX();
        var atPos = Level.Instance.Get(pos);
        if (atPos != null)
        {
            bool class1 = atPos is Player, class2 = this is Player;
            if (class1 != class2)
            {
                atPos.TakeDmg(this);
                return true;
            }
            else
            {
                return false;
            }
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

        if (v)
        {
            if (_pe == null)
            {
                _pe = PushedEffect.Create();
                _pe.transform.parent = transform;
                _pe.transform.position = transform.position;
            }
        }
        else
        {
            Destroy(_pe);
        }

        Scale = v ? 0.7f : 1f;
    }

    private Vector3 _fPos, _tPos;
    public Vector3 Position
    {
        get { return _tPos; }
        set
        {
            _tPos = value;
            _fPos = transform.position;
        }
    }

    private float _fScale = 1, _tScale = 1;

    public float Scale
    {
        get { return _tScale; }
        set
        {
            _tScale = value;
            _fScale = transform.localScale.x;
        }
    }

    private IEnumerator Interpolate()
    {
        transform.position = _fPos;
        transform.localScale = new Vector3(_fScale, _fScale, 1);
        
        
        var t = 0f;
        while (t < AnimationWindow)
        {
            t += Time.deltaTime;
            
            var x = Utils.Interpolate(_fPos.x, _tPos.x, AnimationWindow, t);
            var y = Utils.Interpolate(_fPos.y, _tPos.y, AnimationWindow, t);
            transform.position = new Vector3(x, y, 0);

            var newScale = Utils.Interpolate(_fScale, _tScale, AnimationWindow, t);
            transform.localScale = new Vector3(newScale, newScale, 1);
            
            yield return null;
        }
        
        transform.position = _tPos;
        _fPos = _tPos;
        
        transform.localScale = new Vector3(_tScale, _tScale, 1);
        _fScale = _tScale;
    }

    private IEnumerator InterpolateScale()
    {
        if (_fScale == _tScale) yield break;
        transform.localScale = new Vector3(_fScale, _fScale, 1);
        var t = AnimationWindow;
        var speed = (_tScale - _fScale) / AnimationWindow;
        while (t > 0)
        {
            t -= Time.deltaTime;
            var delta = t < 0 ? Time.deltaTime + t : Time.deltaTime;
            var newScale = transform.localScale.x + speed * delta;
            transform.localScale = new Vector3(newScale, newScale, 1);
            yield return null;
        }
        transform.localScale = new Vector3(_tScale, _tScale, 1);
        _fScale = _tScale;
    }

    public void PlayAnimations()
    {
        StopCoroutine(Interpolate());
        StartCoroutine(Interpolate());
    }

    public virtual bool TickUpdate()
    {
        return true;
    }

    public virtual void Die()
    {
        Level.Instance.Clear(Position.IntX());
        Destroy(gameObject);
    }
}