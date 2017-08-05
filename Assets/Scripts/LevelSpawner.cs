using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TickAction
{
    None, SimpleSquare, Square, Rhombus, Triangle, RhombusSSquare, Save
}

[Serializable]
public struct Event
{
    public int Tick;
    public TickAction TickAction;
    public bool CountDistance;
}

public class EnemyPattern
{
    private int _length = 3;
    private readonly List<Prefab> _left = new List<Prefab>(), _right = new List<Prefab>();
    private readonly List<int> _leftHp = new List<int>(), _rightHp = new List<int>();

    public EnemyPattern AddLeft(Prefab p, int hp = 1)
    {
        _left.Add(p);
        _leftHp.Add(hp);
        return this;
    }
    public EnemyPattern AddRight(Prefab p, int hp = 1)
    {
        _right.Add(p);
        _rightHp.Add(hp);
        return this;
    }

    public EnemyPattern SetLength(int l)
    {
        _length = l;
        return this;
    }

    private int _li, _ri, _produced;
    public GameObject GetNext()
    {
        GameObject go = null;
        if (_produced % 2 != 0)
        {
            if (_left[_li] != null)
            {
                go = _left[_li].Instantiate();
                go.transform.position = new Vector3(-LevelSpawner.Distance, 0, 0);
                go.GetComponent<Unit>().HP = _leftHp[_li];
            }
            _li = _li + 1 % _left.Count;
        }
        else
        {
            if (_right[_ri] != null)
            {
                go = _right[_ri].Instantiate();
                go.transform.position = new Vector3(LevelSpawner.Distance, 0, 0);
                go.GetComponent<Unit>().HP = _rightHp[_ri];
            }
            _ri = _ri + 1 % _right.Count;
        }
        _produced++;
        return go;
    }

    public bool Ended()
    {
        return _produced >= _length * 2;
    }

    public void Reset()
    {
        _produced = 0;
        _li = 0;
        _ri = 0;
    }
}

public class LevelSpawner
{
    public const int Distance = 8;

    private static readonly Prefab Square = BasicEnemy.Prefab;
    private static readonly Prefab Triangle = TriangleEnemy.Prefab;
    private static readonly Prefab Rhombus = RhombusEnemy.Prefab;

    private readonly List<List<EnemyPattern>> _patterns;

    public LevelSpawner()
    {
        _patterns = new List<List<EnemyPattern>>();
        _patterns.Add(new List<EnemyPattern>
        {
            new EnemyPattern().AddLeft(Square).AddRight(Square).AddLeft(Rhombus, 2).AddRight(Rhombus, 2).SetLength(2),
            new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetLength(2),
            new EnemyPattern().AddLeft(Square).AddRight(Rhombus, 2).SetLength(4),
        });
        _patterns.Add(new List<EnemyPattern>
        {
            new EnemyPattern().AddRight(Triangle, 2).AddLeft(null).SetLength(4),
        });
        _patterns.Add(new List<EnemyPattern>
        {
            new EnemyPattern().AddRight(Rhombus, 3).AddLeft(Rhombus, 3).SetLength(4),
        });
        _curPattern = _patterns[_cl][_ci];
        Utils.InvokeDelayed(() =>
        {
            if (_cl < _patterns.Count - 1)
                _cl++;
        }, 15, null, true);
    }

    private EnemyPattern _curPattern;
    private int _ci = 0, _cl = 0;
    public void TickUpdate()
    {
        if (Level.Ticks % 3 != 0) return;
        _curPattern.GetNext();
        if (!_curPattern.Ended()) return;
        
        _curPattern.Reset();
        _ci += Random.Range(1, _patterns[_cl].Count - 1);
        _ci %= _patterns[_cl].Count;
        _curPattern = _patterns[_cl][_ci];
    }
}