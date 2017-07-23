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
        GameObject go;
        if (_produced % 2 == 0)
        {
            go = _left[_li].Instantiate();
            go.transform.position = new Vector3(LevelSpawner.Distance, 0, 0);
            go.GetComponent<Unit>().HP = _leftHp[_li];
            _li = _li + 1 % _left.Count;
        }
        else
        {
            go = _right[_ri].Instantiate();
            go.transform.position = new Vector3(-LevelSpawner.Distance, 0, 0);
            go.GetComponent<Unit>().HP = _rightHp[_ri];
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
    public TickAction CurAction = TickAction.None;
    public List<Event> TickEvents;
    
    private static readonly Prefab Square = new Prefab("SquareEnemy");
    private static readonly Prefab Triangle = new Prefab("TriangleEnemy");
    private static readonly Prefab Rhombus = new Prefab("RhombusEnemy");

    private readonly List<EnemyPattern> _patterns;

    public LevelSpawner()
    {
        _patterns = new List<EnemyPattern>
        {
            new EnemyPattern().AddLeft(Square).AddRight(Square).AddLeft(Rhombus, 2).AddRight(Rhombus, 2).SetLength(2),
            new EnemyPattern().AddLeft(Square, 2).AddRight(Square, 2).SetLength(2)
        };
        _ci = Random.Range(0, _patterns.Count);
        _curPattern = _patterns[_ci];
    }

    private EnemyPattern _curPattern;
    private int _ci;
    public void TickUpdate()
    {
        if (Level.Ticks % 3 != 0) return;
        _curPattern.GetNext();
        if (_curPattern.Ended())
        {
            _curPattern.Reset();
            _ci += Random.Range(1, _patterns.Count - 1);
            _ci %= _patterns.Count;
            _curPattern = _patterns[_ci];
        }
    }

    private GameObject PlaceGo(Prefab l, Prefab r = null)
    {
        GameObject go = null;
        if (Level.Ticks % 3 == 0)
        {
            if (Level.Ticks / 3 % 2 == 0)
            {
                go = r == null ? l.Instantiate() : r.Instantiate();
                go.transform.position = new Vector3(Distance, 0, 0);
            }
            else
            {
                go = l.Instantiate();
                go.transform.position = new Vector3(-Distance, 0, 0);
            }
        }
        return go;
    }
}