using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid
{
    private readonly Unit[] _units;
    private readonly List<Unit>[] _pushed;
    private readonly int _gridOffset;

    public Grid(int size)
    {
        _units = new Unit[size];
        _pushed = new List<Unit>[size];
        _gridOffset = size / 2;
    }

    public Unit Get(int pos)
    {
        pos += _gridOffset;
        return _units[pos];
    }

    public void Clear(int pos)
    {
        pos += _gridOffset;
        _units[pos] = Pop(pos);
    }

    private Unit Pop(int pos)
    {
        if (_pushed[pos] == null || _pushed[pos].Count == 0) return null;
        var popped = _pushed[pos][0];
        _pushed[pos].RemoveAt(0);
        for (var i = 0; i < _pushed[pos].Count; i++)
        {
            var unit = _pushed[pos][i];
            unit.transform.position = new Vector3(unit.Position, i + 1, 0);
        }
        popped.transform.position = new Vector3(popped.Position, 0, 0);
        popped.JustPopped = true;
        popped.SetPushedVisuals(false);
        return popped;
    }

    private void Push(int pos, Unit unit)
    {
        if (_pushed[pos] == null)
        {
            _pushed[pos] = new List<Unit>();
        }
        _pushed[pos].Add(unit);
        unit.transform.position = new Vector3(unit.Position, _pushed[pos].Count);
        unit.SetPushedVisuals(true);
    }

    private void PushFront(int pos, Unit unit)
    {
        if (_pushed[pos] == null)
        {
            _pushed[pos] = new List<Unit>();
        }
        _pushed[pos].Insert(0, unit);
        for (var i = 0; i < _pushed[pos].Count; i++)
        {
            var u = _pushed[pos][i];
            u.transform.position = new Vector3(u.Position, i + 1, 0);
        }
    }

    public void Set(int pos, Unit unit)
    {
        var prevPos = unit.Position + _gridOffset;
        pos += _gridOffset;

        if (prevPos != pos)
        {
            _units[prevPos] = Pop(prevPos);
        }
        
        if (_units[pos] != null)
        {
            if (unit is Player)
            {
                PushFront(pos, _units[pos]);
            }
            else
            {
                Push(pos, unit);
            }
            return;
        }

        _units[pos] = unit;
    }

    public List<Unit> GetAllUnits()
    {
        return _units.Where(unit => unit != null).ToList();
    }
}
