using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	public static Level Instance { get; private set; }
	private const int Size = 100;
	private readonly Grid _grid = new Grid(Size);
	private static readonly Prefab _gridSquare = new Prefab("GridSquare");

	private void Awake()
	{
		Instance = this;
		const int offset = Size / 2;
		for (var i = -offset; i < offset; i++)
		{
			var square = _gridSquare.Instantiate();
			square.transform.position = new Vector3(i, -0.7f, 0);
		}
	}

	public void Move(int pos, Unit unit)
	{
		_grid.Set(pos, unit);
	}

	public bool Attack(int pos, Unit unit)
	{
		if (_grid.Get(pos) == null) return false;
		if (_grid.Get(pos) == unit) return false;
		_grid.Get(pos).TakeDmg(unit);
		return true;
	}

	public Unit Get(int pos)
	{
		return _grid.Get(pos);
	}

	public void Clear(int pos)
	{
		_grid.Clear(pos);
	}

	public void TickUpdate()
	{
		var units = _grid.GetAllUnits();
		var ticked = false;
		while (true)
		{
			var filtered = new List<Unit>();
			foreach (var unit in units)
			{
				var result = unit.TickUpdate();
				if (result)
				{
					ticked = true;
					continue;
				}
				filtered.Add(unit);
			}
			if (filtered.Count == 0 || !ticked) break;
			ticked = false;
			units = filtered;
		}
		foreach (var unit in _grid.GetAllUnitsWithPushed())
		{
			unit.PlayAnimations();
		}
	}

	public void InitPos(Unit unit)
	{
		_grid.Set(unit.Position.IntX(), unit);
	}
}
