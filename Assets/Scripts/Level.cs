using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	public static Level Instance { get; private set; }
	private readonly Grid _grid = new Grid(100);

	private void Awake()
	{
		Instance = this;
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
	}

	public void InitPos(Unit unit)
	{
		_grid.Set(unit.Position, unit);
		unit.transform.position = new Vector3(unit.Position, unit.transform.position.y, 0);
	}
}
