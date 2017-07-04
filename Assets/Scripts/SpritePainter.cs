using System;
using UnityEngine;

public class SpritePainter : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Camera _camera;
    private Color? _initial;
    private readonly Group<ColorChanger> _changers = new Group<ColorChanger>();

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        if (_sprite == null)
        {
            _camera = GetComponent<Camera>();
        }
    }

    public void Paint(Color c, float? time, bool smooth)
    {
        Paint(new ColorChanger(c, time, smooth));
    }

    public void Paint(Color c)
    {
        Paint(c, null, false);
    }

    public void Paint(ColorChanger cc)
    {
        if (_initial == null)
        {
            _initial = _sprite != null ? _sprite.color : _camera.backgroundColor;
        }
        _changers.Add(cc);
    }

    public void Clear()
    {
        _changers.Clear();
    }

    private void Update()
    {
        _changers.Refresh();
        if (_initial == null)
            return;
        var c = _initial.Value;
        foreach (var a in _changers)
        {
            a.Update();
            if (a.Time != null && a.Time < 0)
                _changers.Remove(a);
            c = a.Change(c);
        }
        if (_sprite != null)
            _sprite.color = c;
        else
            _camera.backgroundColor = c;
    }
}

public class ColorChanger
{
    private readonly bool _smooth;
    protected float InitialTime;
    public float? Time;
    protected Color Color;

    public ColorChanger(Color c, float? time, bool smooth)
    {
        Time = time;
        _smooth = smooth;
        if (time != null)
            InitialTime = time.Value;
        Color = c;
    }

    public Color Change(Color c)
    {
        float smoothValue = 1;
        if (Time != null && _smooth)
            smoothValue = Time.Value / InitialTime;
        return Color.Lerp(c, Color, smoothValue);
    }

    public virtual void Update()
    {
        if (Time != null)
            Time -= UnityEngine.Time.deltaTime;
    }
}

public class PulsingChanger : ColorChanger
{
    private readonly Color _c1, _c2;
    private float _t;
    private readonly float _period;

    public PulsingChanger(Color c1, Color c2, float? time, float period = 0.5f) : base(c1, time, false)
    {
        _c1 = c1;
        _c2 = c2;
        _period = period;
    }

    private bool _pos = true;

    public override void Update()
    {
        base.Update();
        _t += (_pos ? 1 : -1) * UnityEngine.Time.deltaTime / _period;
        if (_t > 1)
        {
            _pos = false;
            _t = 1;
        }
        if (_t < 0)
        {
            _pos = true;
            _t = 0;
        }
        Color = Color.Lerp(_c1, _c2, _t);
    }
}

public class FadeInChanger : ColorChanger
{
    private readonly float _lifeTime;

    public FadeInChanger(Color c, float lifeTime, float fadePeriod) : base(c, 0, true)
    {
        InitialTime = fadePeriod;
        _lifeTime = lifeTime;
    }

    public override void Update()
    {
        Time += UnityEngine.Time.deltaTime;
        if (Time > _lifeTime)
        {
            Time = -1f;
        }
    }
}