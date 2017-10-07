using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
	public static Level Instance { get; protected set; }
	public static int Size = 15;
	private Grid _grid = new Grid(15);
	public static float TickTime = 0.5f;
	public static int Ticks = -1;
	public static bool Updating = true;
	public static bool GameOver = false;
	public static bool Spawning = true;
	public static int SaveTicks = -1;
	public static bool IsFirstStart = true;
	public static int CurrentLevel = 0;
	public int StartTicks = -1;
	public const int LevelsAmount = 6; 
	private LevelSpawner _levelSpawner;
	private AudioSource _audioSource;
	public Text RestartText;
	public Text QuitText;
	public Text ControlsText;
	public Text TimeText;
	public AudioClip Char, Deicide;
	public float MusicStart, MusicDelay;

	public Level()
	{
		Instance = this;
	}

	[NonSerialized] public Prefab Killer = null;
	[NonSerialized] public Unit KillerUnit = null;
	[NonSerialized] public int KillerHP;
	
	public void Restart(float delay = 1.75f)
	{
		Updating = false;
		Ticks = -1;
		Invoke("InvokeRestart", delay);
	}

	private void InvokeRestart()
	{
		
		Updating = true;
	}

	private void Awake()
	{
		Prefab.TouchStatics();
		Prefab.PreloadPrefabs();
		_audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		_levelSpawner = new LevelSpawner(CurrentLevel);
		var size = LevelSpawner.Distance * 2 + 1;
		_grid = new Grid(size);
		Player.Prefab.Instantiate();
		Updating = true;
		GameOver = false;
		Spawning = true;
		_started = false;
		Pattern.Instance.Reset();
		var offset = LevelSpawner.Distance;
		GridMarks.Instance.SetSize(offset);
		GridMarks.Instance.SetBorderHandlers(() =>
			{
				Player.Instance.DieEvent = () => { };
				Player.Instance.TakeDmg(Player.Instance, 999);
			},
			() =>
			{
				Player.Instance.DieEvent = () => { };
				Player.Instance.TakeDmg(Player.Instance, 999);
			});
		GridMarks.Instance.SetBorders(-1, 1);

		var delay = IsFirstStart ? MusicDelay : 0;

		CancelInvoke("TickUpdate");
		InvokeRepeating("TickUpdate", delay, TickTime);
		if (SaveTicks != -1)
		{
			StartTicks = SaveTicks;
		}
		_audioSource.volume = 0f;
		Utils.Animate(0f, 1f, 0.9f, (v) => _audioSource.volume += v);
		_audioSource.time = (StartTicks + 1) * TickTime + MusicStart - delay;
		_audioSource.Play();
		Ticks = StartTicks;
		
		if (!IsFirstStart) return;
		IsFirstStart = false;
		var c = ControlsText.color;
		c.a = 1;
		var ut = ControlsText.GetComponent<UnitedTint>();
		ut.Color = c;
		c = new Color(c.r, c.g, c.b, 0);
		Utils.Animate(1f, 0f, 3f, (v) =>
		{
			c.a = v;
			ut.Color = c;
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
	public Action StartAction, GameOverAction = () => { }, ExitGameOverAction = () => { };

	public void TickUpdate()
	{
		if (!Updating)
		{
			return;
		}
		if (!_started && !GameOver)
		{
			StartAction();
			_levelSpawner.StartAction();
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
		var score = TimeText.text;
		if (string.IsNullOrEmpty(PlayerData.Instance.Scores[CurrentLevel]) || float.Parse(score) > float.Parse(PlayerData.Instance.Scores[CurrentLevel]))
		{
			PlayerData.Instance.Scores[CurrentLevel] = score;
			Saves.Save();
			WebUtils.SendScore(CurrentLevel);
		}
		Updating = false;
		GameOver = true;
		GridMarks.Instance.HandlerLeft += () =>
		{
			if (!GameOver) return;
			ExitGameover();
			Utils.InvokeDelayed(OnEnable, GOAnimationTime / 4);
		};
		GridMarks.Instance.HandlerRight += () =>
		{
			CameraScript.Instance.SwitchScene(() =>
			{
				WebUtils.FetchScores();
				ExitGameover();
				gameObject.SetActive(false);
				Menu.Instance.gameObject.SetActive(true);
				Menu.Instance.RefreshItems(true);
			});
		};
		
		Utils.Animate(1f, 0f, 0.5f, (v) => _audioSource.volume += v);
//		CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.43f, 0f, 0.01f), GOAnimationTime / 2, true);
		Utils.Animate(Camera.main.backgroundColor, Color.black, GOAnimationTime / 2, (v) => Camera.main.backgroundColor += v);
		Utils.Animate(UnitedTint.Tint, Color.white, GOAnimationTime / 2, (v) => UnitedTint.Tint = v, null, true);
		Pattern.Instance.Reset();
		Utils.InvokeDelayed(KillEverything, GOAnimationTime / 2);
		var rtut = RestartText.GetComponent<UnitedTint>();
		var qtut = QuitText.GetComponent<UnitedTint>();
		var ct = rtut.Color;
		ct = new Color(ct.r, ct.g, ct.b, 0);
		var tt = TimeText.color;
		Utils.InvokeDelayed(() =>
		{
			GridMarks.Instance.SetBorders(-3, 1);
			Utils.Animate(0f, 1f, GOAnimationTime / 4, (v) =>
			{
				CameraScript.Instance.InvProgress += v;
				ct.a += v;
				rtut.Color = ct;
				qtut.Color = ct;
				tt.r = 1 - ct.a;
				tt.g = 1 - ct.a;
				tt.b = 1 - ct.a;
				TimeText.color = tt;
			});
		}, GOAnimationTime * 0.75f);
		Utils.InvokeDelayed(() =>
		{
//			Camera.main.GetComponent<SpritePainter>().Paint(Color.black);
			RespawnGOUnits();
			if (Killer != null) RespawnKiller();
			GameOverAction();
		}, GOAnimationTime);
	}

	public void RespawnGOUnits()
	{
		KillEverything();
		CameraScript.Instance.SwitchProgress = 0;
		Utils.InvokeDelayed(() =>
		{
			var pgo = Player.Prefab.Instantiate();
			var p = pgo.GetComponent<Player>();
			p.GameOverInstance = true;
			p.DieEvent = () =>
			{
				if (GameOver) Utils.InvokeDelayed(RespawnGOUnits, 1f);
			};
		}, 0.3f);
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
		KillerUnit = unit;
		unit.HP = KillerHP;
		unit.DieEvent += () =>
		{
			Utils.InvokeDelayed(RespawnKiller, 1f);
		};
		go.transform.position = new Vector3(-2, 0, 0);
		go.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
	}

	public void ExitGameover()
	{
		GameOver = false;
		Updating = true;
		Spawning = true;
		ExitGameOverAction();
		Player.Instance.DieEvent = () => { };
		KillEverything();
		var rtut = RestartText.GetComponent<UnitedTint>();
		var qtut = QuitText.GetComponent<UnitedTint>();
		var ct = rtut.Color;
		ct = new Color(ct.r, ct.g, ct.b, 1);
		var tt = TimeText.color;
		
		Utils.Animate(1f, 0f, GOAnimationTime / 4, (v) =>
		{
			CameraScript.Instance.InvProgress += v;
			ct.a += v;
			rtut.Color = ct;
			qtut.Color= ct;
			tt.r = 1 - ct.a;
			tt.g = 1 - ct.a;
			tt.b = 1 - ct.a;
			TimeText.color = tt;

		});
		CancelInvoke("TickUpdate");
	}

	public void KillEverything()
	{
		GetAllUnits().ForEach((u) =>
		{
			u.TakeDmg(null, 999);
		});
	}
}
