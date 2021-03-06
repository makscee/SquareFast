﻿using UnityEngine;

public class BeatBGEffect : MonoBehaviour
{
    private float _t;
    private const float _lifeTime = 1.5f;
    private SpriteRenderer _sr;
    private UnitedTint _ut;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _ut = GetComponent<UnitedTint>();
    }
    
    private void Update()
    {
        _t += Time.deltaTime;
        var st = Utils.Interpolate(0f, 15f, _lifeTime, _t, InterpolationType.InvSquare);
        transform.localScale += new Vector3(st, st);
        var at = 1f - _t / _lifeTime;
        _ut.Color = _ut.Color.ChangeAlpha(at);
        if (_t > _lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private static Prefab[] _prefab = new Prefab[] {
        new Prefab("BeatBGRhombus"),
        new Prefab("BeatBGSquare"),
        new Prefab("BeatBGCircle"),
    };
    public static void Create(int i)
    {
        var go = _prefab[i].Instantiate();
    }
}