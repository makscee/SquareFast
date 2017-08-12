using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
	public static Level Instance { get; private set; }
	public const int Size = LevelSpawner.Distance * 2 + 1;
	private readonly Grid _grid = new Grid(Size);
	private static readonly Prefab GridSquare = new Prefab("GridSquare");
	public static float TickTime = 0.5f;
	public static int Ticks = -1;
	public static bool Updating = true;
	public static bool GameOver = false;
	public static bool Spawning = true;
	public static int SaveTicks = -1;
	public int StartTicks = -1;
	private LevelSpawner _levelSpawner;
	private AudioSource _audioSource;
	public Text ContinueText;
	public Text ControlsText;
	public Text TimeText;

	private GameObject _leftBorder, _rightBorder;

	[NonSerialized] public Prefab Killer = null;
	[NonSerialized] public int KillerHP;
	
	public void Restart(float delay = 1.75f)
	{
		Updating = false;
		Ticks = -1;
		Invoke("InvokeRestart", delay);
	}

	private void InvokeRestart()
	{
		SceneManager.LoadScene(0);
//		Player.Instance.Move(-Player.Instance.Position.IntX());
		Updating = true;
	}

	private static void TouchStatics() {
		var types = new List<Type>
		{
			typeof(HitEffect),
			typeof(PushedEffect),
			typeof(ShieldDieEffect),
			typeof(Unit),
			typeof(SpawnEffect),
			typeof(LevelSpawner),
		};
		foreach (var t in types) 
		{
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor (t.TypeHandle);
		}
	}

	public float OverrideTickTime = 0;

	private void Start()
	{
		_levelSpawner = new LevelSpawner();
	}
	
	private void Awake()
	{
		TouchStatics();
		Prefab.PreloadPrefabs();
		Instance = this;
		const int offset = LevelSpawner.Distance;
		for (var i = -offset; i <= offset; i++)
		{
			var square = GridSquare.Instantiate();
			square.transform.position = new Vector3(i, -0.7f, 0);
			square.transform.SetParent(transform);
		}
		var border = GridSquare.Instantiate();
		_rightBorder = border;
		border.transform.position = new Vector3(1.5f, 0f, 0);
		border.transform.Rotate(0f, 0f, 90f);
		border.transform.SetParent(transform);
		border = GridSquare.Instantiate();
		_leftBorder = border;
		border.transform.position = new Vector3(-1.5f, 0f, 0);
		border.transform.Rotate(0f, 0f, 90f);
		border.transform.SetParent(transform);

		TickTime = 60f / 100f / 3f;
		const float musicStart = 9.7f;
		var delay = Time.time > 0 ? 0 : 2f;

		if (OverrideTickTime > 0)
		{
			TickTime = OverrideTickTime;
		}
		CancelInvoke("TickUpdate");
		InvokeRepeating("TickUpdate", (OverrideTickTime > 0 ? 0 : delay), TickTime);
		if (SaveTicks != -1)
		{
			StartTicks = SaveTicks;
		}
		_audioSource = GetComponent<AudioSource>();
		_audioSource.volume = 0f;
		Utils.Animate(0f, 1f, 0.9f, (v) => _audioSource.volume += v);
		_audioSource.time = (StartTicks + 1) * TickTime + musicStart - delay;
		Ticks = StartTicks;
		
		if (Time.time != 0) return;
		var c = ControlsText.color;
		c.a = 1;
		ControlsText.color = c;
		c = new Color(c.r, c.g, c.b, 0);
		Utils.Animate(1f, 0f, 3f, (v) =>
		{
			c.a = v;
			ControlsText.color = c;
		}, null, true, 2f);
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

	private bool _started;
	public Action StartAction;

	public void TickUpdate()
	{
		if (!Updating)
		{
			return;
		}
		if (!_started && !GameOver)
		{
			StartAction();
			_started = true;
		}
		Ticks++;
		Pattern.Instance.TickUpdate();
		if (Spawning) _levelSpawner.TickUpdate();
		var units = _grid.GetAllUnits();
		var ticked = false;
		foreach (var unit in units)
		{
			unit.TickAnimations();
		}
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

	public List<Unit> GetAllUnits()
	{
		return _grid.GetAllUnitsWithPushed();
	}

	private const float GOAnimationTime = 1f;
	public void EnterGameOver()
	{
		Utils.Animate(1f, 0f, 0.5f, (v) => _audioSource.volume += v);
		CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.43f, 0f, 0.01f), GOAnimationTime / 2, true);
		Updating = false;
		GameOver = true;
		Pattern.Instance.Reset();
		Utils.InvokeDelayed(KillEverything, GOAnimationTime / 2);
		var c = ContinueText.color;
		c = new Color(c.r, c.g, c.b, 0);
		Utils.InvokeDelayed(() =>
		{
			Utils.Animate(0f, 1f, GOAnimationTime / 4, (v) =>
			{
				CameraScript.Instance.InvProgress += v;
				c.a += v;
				ContinueText.color = c;
			});
		}, GOAnimationTime * 0.75f);
		Utils.InvokeDelayed(() =>
		{
			RespawnGOUnits();
			if (Killer != null) RespawnKiller();
		}, GOAnimationTime);
	}

	public void RespawnGOUnits()
	{
		KillEverything();
		_rightBorder.SetActive(false);
		var p = Player.Prefab.Instantiate();
		p.GetComponent<Player>().GameOverInstance = true;
		Updating = true;
		Spawning = false;
		TickTime = 0.5f;
		CancelInvoke("TickUpdate");
		InvokeRepeating("TickUpdate", 0f, TickTime);
	}

	private void RespawnKiller()
	{
		if (!GameOver)
		{
			return;
		}
		var go = Killer.Instantiate();
		var unit = go.GetComponent<Unit>();
		unit.HP = KillerHP;
		unit.DieEvent += () =>
		{
			Utils.InvokeDelayed(RespawnKiller, 1f);
		};
		go.transform.position = new Vector3(5, 0, 0);
		go.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
	}

	public void ExitGameover()
	{
		GameOver = false;
		Updating = true;
		Spawning = true;
		KillEverything();
		var c = ContinueText.color;
		c = new Color(c.r, c.g, c.b, 1);
		Utils.Animate(1f, 0f, GOAnimationTime / 4, (v) =>
		{
			CameraScript.Instance.InvProgress += v;
			c.a += v;
			ContinueText.color = c;
		});
		Restart(GOAnimationTime / 2);
	}

	public void KillEverything()
	{
		GetAllUnits().ForEach((u) =>
		{
			u.TakeDmg(null, 999);
		});
	}
}
