using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
	public static Level Instance { get; private set; }
	private const int Size = 100;
	private readonly Grid _grid = new Grid(Size);
	private static readonly Prefab GridSquare = new Prefab("GridSquare");
	public static float TickTime = 0.5f;
	public static bool Updating = true;

	public void Restart(float delay = 3f)
	{
		Updating = false;
		Debug.Log("restarting");
		Invoke("InvokeRestart", delay);
	}

	private void InvokeRestart()
	{
		SceneManager.LoadScene(0);
		Updating = true;
	}

	private void TouchStatics() {
		List<Type> types = new List<Type>
		{
			typeof(HitEffect),
			typeof(PushedEffect)
		};
		foreach (var t in types) 
		{
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor (t.TypeHandle);
		}
	}

	private void Awake()
	{
		TouchStatics();
		Prefab.PreloadPrefabs();
		Instance = this;
		const int offset = Size / 2;
		for (var i = -offset; i < offset; i++)
		{
			var square = GridSquare.Instantiate();
			square.transform.position = new Vector3(i, -0.7f, 0);
			square.transform.SetParent(transform);
		}
		var border = GridSquare.Instantiate();
		border.transform.position = new Vector3(1.5f, 0f, 0);
		border.transform.Rotate(0f, 0f, 90f);
		border.transform.SetParent(transform);
		border = GridSquare.Instantiate();
		border.transform.position = new Vector3(-1.5f, 0f, 0);
		border.transform.Rotate(0f, 0f, 90f);
		border.transform.SetParent(transform);
		InvokeRepeating("TickUpdate", TickTime, TickTime);
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
		if (!(Get(pos) is Player))
		{
			EnemiesCount--;
		}
		_grid.Clear(pos);
	}

	public int EnemiesCount { get; private set; }

	public void TickUpdate()
	{
		if (!Updating)
		{
			return;
		}
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
		_grid.Set(unit.Position.IntX(), unit);
		if (!(unit is Player))
		{
			EnemiesCount++;
		}
	}
}
