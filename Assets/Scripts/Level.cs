using System;
using UnityEngine;

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

	public bool MoveOrAttack(int pos, Unit unit)
	{
		if (Move(pos, unit)) return true;
		_grid.Get(pos).TakeDmg(unit);
		return false;
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
