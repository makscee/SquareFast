﻿using UnityEngine;

public class Level : MonoBehaviour
{
	public static Level Instance { get; private set; }
	private readonly Grid _grid = new Grid(100);

	private void Awake()
	{
		Instance = this;
	}

	public bool Move(int pos, Unit unit)
	{
		return _grid.TrySet(pos, unit);
	}

	public bool Attack(int pos, Unit unit)
	{
		if (_grid.Get(pos) == null) return false;
		if (_grid.Get(pos) == unit) return false;
		_grid.Get(pos).TakeDmg(unit);
		return true;
	}

	public bool MoveOrAttack(int pos, Unit unit)
	{
		if (Move(pos, unit)) return true;
		_grid.Get(pos).TakeDmg(unit);
		return false;
	}

	public void Clear(int pos)
	{
		_grid.Set(pos, null);
	}

	public void TickUpdate()
	{
		var units = _grid.GetAllUnits();
		foreach (var unit in units)
		{
			if (unit is Player) continue;
			unit.TickUpdate();
		}
	}

	public void InitPos(Unit unit)
	{
		while (!_grid.TrySet(unit.Position, unit))
		{
			var dir = unit.Position > 0 ? 1 : -1;
			unit.Position += dir;
		}
		unit.transform.position = new Vector3(unit.Position, 0, 0);
	}
}
