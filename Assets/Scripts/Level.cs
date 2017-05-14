using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	public static Level Instance { get; private set; }
	private readonly Grid _grid = new Grid();

	private void Awake()
	{
		Instance = this;
	}

	public void Move(int relDir, Unit unit)
	{
		var pos = unit.Position + relDir;
		_grid.SetOrReplace(pos, unit);
	}

	public void InitPos(Unit unit)
	{
		if (_grid.Get(unit.Position))
		{
			unit.Position++;
			InitPos(unit);
		}
		else
		{
			_grid.SetOrReplace(unit.Position, unit);
		}
	}
}
