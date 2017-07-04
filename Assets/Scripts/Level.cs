using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
	public enum Layouts
	{
		None, SimpleSquare, Square, Rhombus, Triangle
	}

	private static Layouts _layout;
	public static Level Instance { get; private set; }
	private const int Size = 100;
	private readonly Grid _grid = new Grid(Size);
	private static readonly Prefab GridSquare = new Prefab("GridSquare");
	public static float TickTime = 0.5f;
	public static bool Updating = true;
	
	private readonly Prefab _square = new Prefab("SquareEnemy");
	private readonly Prefab _triangle = new Prefab("TriangleEnemy");
	private readonly Prefab _rhombus = new Prefab("RhombusEnemy");

	private static readonly List<Tuple<Layouts, int>> Levels = new List<Tuple<Layouts, int>>()
	{
		new Tuple<Layouts, int>(Layouts.SimpleSquare, 1),
		new Tuple<Layouts, int>(Layouts.SimpleSquare, 2),
		new Tuple<Layouts, int>(Layouts.Square, 1),
		new Tuple<Layouts, int>(Layouts.Square, 2),
		new Tuple<Layouts, int>(Layouts.Square, 3),
		new Tuple<Layouts, int>(Layouts.Square, 4),
		new Tuple<Layouts, int>(Layouts.Rhombus, 1),
		new Tuple<Layouts, int>(Layouts.Rhombus, 2),
		new Tuple<Layouts, int>(Layouts.Rhombus, 3),
		new Tuple<Layouts, int>(Layouts.Rhombus, 4),
		new Tuple<Layouts, int>(Layouts.Triangle, 1),
		new Tuple<Layouts, int>(Layouts.Triangle, 2),
		new Tuple<Layouts, int>(Layouts.Triangle, 3),
		new Tuple<Layouts, int>(Layouts.Triangle, 4),
	};

	private static Tuple<Layouts, int> _curLevel; 
	public void NextLevel()
	{
		if (Levels.Count == 0)
		{
			CameraScript.Instance.GetComponent<SpritePainter>().Paint(new FadeInChanger(Color.white, 999, 4f));
			return;
		}
		_curLevel = Levels[0];
		Levels.RemoveAt(0);
		CounterScript.Instance.Set((int)_curLevel.Item1, _curLevel.Item2);
		Restart();
	}
	
	public void Restart(float delay = 1.75f)
	{
		Updating = false;
		Debug.Log("restarting");
		Invoke("InvokeRestart", delay);
	}

	private void InvokeRestart()
	{
		SceneManager.LoadScene(0);
//		Player.Instance.Move(-Player.Instance.Position.IntX());
		Updating = true;
	}

	private void InitEnemies()
	{
		Debug.Log(_layout);
		switch (_layout)
		{
			case Layouts.None: break;
			case Layouts.SimpleSquare:
			{
				for (var i = 3; i <= 7; i += 2)
				{
					var go = _square.Instantiate();
					go.transform.position = new Vector3(i, 0, 0);
					go.GetComponent<Unit>().Shielded = false;
					go = _square.Instantiate();
					go.transform.position = new Vector3(-i - 1, 0, 0);
					go.GetComponent<Unit>().Shielded = false;
				}
				break;
			}
			case Layouts.Square:
				for (var i = 3; i <= 9; i += 2)
				{
					var go = _square.Instantiate();
					go.transform.position = new Vector3(i, 0, 0);
					go = _square.Instantiate();
					go.transform.position = new Vector3(-i - 1, 0, 0);
				}
				break;
			case Layouts.Triangle:
				for (var i = 3; i <= 9; i += 2)
				{
					var go = _triangle.Instantiate();
					go.transform.position = new Vector3(i, 0, 0);
				}
				break;
			case Layouts.Rhombus:
			{
				for (var i = 3; i <= 9; i += 2)
				{
					var go = _rhombus.Instantiate();
					go.transform.position = new Vector3(i, 0, 0);
					go = _rhombus.Instantiate();
					go.transform.position = new Vector3(-i - 1, 0, 0);
				}
				break;
			}
		}
	}

	private static void TouchStatics() {
		var types = new List<Type>
		{
			typeof(HitEffect),
			typeof(PushedEffect),
			typeof(ShieldDieEffect),
			typeof(Unit)
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
		if (_curLevel == null)
		{
			_curLevel = Levels[0];
			Levels.RemoveAt(0);
		}
		TickTime = 0.5f / (float) Math.Pow(1.5f, _curLevel.Item2 - 1);
		Debug.Log("new tt " + TickTime);
		_layout = _curLevel.Item1;
		InitEnemies();
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
