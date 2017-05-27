using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA;

public class Grid
{
    private readonly Unit[] _units;
    private readonly int _gridOffset;

    public Grid(int size)
    {
        _units = new Unit[size];
        _gridOffset = size / 2;
    }

    public bool TrySet(int pos, Unit unit)
    {
        var prevPos = unit.Position + _gridOffset;
        pos += _gridOffset;
        if (_units[pos] != null && _units[pos] != unit)
        {
            return false;
        }
        _units[prevPos] = null;
        _units[pos] = unit;
        return true;
    }

    public Unit Get(int pos)
    {
        pos += _gridOffset;
        return _units[pos];
    }

    public void Set(int pos, Unit unit)
    {
        pos += _gridOffset;
        _units[pos] = unit;
    }

    public List<Unit> GetAllUnits()
    {
        return _units.Where(unit => unit != null).ToList();
    }
}
