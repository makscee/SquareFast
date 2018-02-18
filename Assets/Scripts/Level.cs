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
	public static bool NextLevelStart = false;
	public static int SaveTicks = -1;
	public static bool IsFirstStart = true;
	public static int CurrentLevel = 0;
	public int StartTicks = -1;
	public const int LevelsAmount = 7; 
	private LevelSpawner _levelSpawner;
	private AudioSource _audioSource;
	public Text TimeText;
	public AudioClip L1, L2, Over;
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

	public void OnEnable()
	{
		TickAction = () => { };
		_levelSpawner = new LevelSpawner(CurrentLevel);
		_grid = new Grid(30);
		if (!NextLevelStart)
		{
			Player.Prefab.Instantiate();
		}
		else
		{
			InitPos(Player.Instance);
		}
		
		Updating = true;
		GameOver = false;
		Spawning = true;
		_started = false;
		var offset = LevelSpawner.Distance;
		Action a = () =>
		{
		};
		GridMarks.Instance.Set("", "", -1, 1, -offset, offset, a, a, true, true);

		var delay = IsFirstStart ? MusicDelay : 0;

//		CancelInvoke("TickUpdate");
//		InvokeRepeating("TickUpdate", delay, TickTime);
		_ticking = false;
		Utils.InvokeDelayed(() => _ticking = true, delay);
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
//		if (!(Get(pos) is Player))
//		{
//			EnemiesCount--;
//		}
		_grid.Clear(pos);
	}

//	public int EnemiesCount { get; private set; }

	private bool _started;
	public Action StartAction, TickActionPerm = () => { }, TickAction = () => { }, GameOverStartAction = () => { }, GameOverAction = () => { }, ExitGameOverAction = () => { };

	private float _sampleTime = 0f;
	private bool _ticking = false;
	private void Update()
	{
		if (!_ticking) return;
		var newST= 1.0f *_audioSource.timeSamples / 44100;
		if (Math.Floor(newST / TickTime) > Math.Floor(_sampleTime / TickTime))
		{
			TickUpdate();
		}
		_sampleTime = newST;
	}
	
	public void TickUpdate()
	{
		if (!Updating)
		{
			return;
		}
		if (!_started && !GameOver)
		{
			if (!NextLevelStart)
			{
				StartAction();
			}
			else
			{
				NextLevelStart = false;
			}
			_levelSpawner.StartAction();
			_started = true;
		}
		Ticks++;
		TickActionPerm();
		TickAction();
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
//		if (!(unit is Player))
//		{
//			EnemiesCount++;
//		}
	}

	public List<Unit> GetAllUnits()
	{
		return _grid.GetAllUnitsWithPushed();
	}

	private const float GOAnimationTime = 1f;
	public void EnterGameOver()
	{
		GameOverStartAction();
		var score = TimeText.text;
		if (string.IsNullOrEmpty(PlayerData.Instance.Scores[CurrentLevel]) || float.Parse(score) > float.Parse(PlayerData.Instance.Scores[CurrentLevel]))
		{
			PlayerData.Instance.Scores[CurrentLevel] = score;
			Saves.Save();
			WebUtils.SendScore(CurrentLevel);
		}
		Updating = false;
		GameOver = true;
		
		Utils.Animate(1f, 0f, 0.5f, (v) => _audioSource.volume += v);
		Utils.InvokeDelayed(() =>
		{
			_audioSource.clip = Over;
			_audioSource.Play();
			Utils.Animate(0f, 0.5f, 2f, (v) => _audioSource.volume += v);
		}, 0.5f);
//		CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.43f, 0f, 0.01f), GOAnimationTime / 2, true);
		Utils.Animate(Camera.main.backgroundColor, Color.black, GOAnimationTime / 2, (v) => Camera.main.backgroundColor += v);
		Utils.Animate(UnitedTint.Tint, Color.white, GOAnimationTime / 2, (v) => UnitedTint.Tint = v, null, true);
		Pattern.Instance.Reset();
		Utils.InvokeDelayed(() => KillEverything(), GOAnimationTime / 2);
		var gm = GridMarks.Instance;
		var rtut = gm.LeftText.GetComponent<UnitedTint>();
		var qtut = gm.RightText.GetComponent<UnitedTint>();
		var ct = rtut.Color;
		ct = new Color(ct.r, ct.g, ct.b, 0);
		var tt = TimeText.color;
		Utils.InvokeDelayed(() =>
		{
			gm.Set("< RESTART", "QUIT >", -3, 1, -3, 1, () =>
			{
				if (!GameOver) return;
				ExitGameover();
				Utils.InvokeDelayed(OnEnable, GOAnimationTime / 4);
			}, () =>
			{
				CameraScript.Instance.SwitchScene(() =>
				{
					WebUtils.FetchScores();
					ExitGameover();
					gameObject.SetActive(false);
					Menu.Instance.gameObject.SetActive(true);
					Menu.Instance.RefreshItems(true);
				});
			});
			
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
			RespawnGOUnits();
			RespawnKiller();
			GameOverAction();
		}, GOAnimationTime);
	}

	public void RespawnGOUnits()
	{
		KillEverything();
		CameraScript.Instance.SwitchProgress = 0;
		var pgo = Player.Prefab.Instantiate();
		var p = pgo.GetComponent<Player>();
		p.GameOverInstance = true;
		p.DieEvent = () =>
		{
			if (GameOver) Utils.InvokeDelayed(RespawnGOUnits, 1f);
		};
		Updating = true;
		Spawning = false;
		TickTime = 0.5f;
		_sampleTime = 0f;
//		CancelInvoke("TickUpdate");
//		InvokeRepeating("TickUpdate", 0f, TickTime);
	}

	private void RespawnKiller()
	{
		if (!GameOver)
		{
			return;
		}
		if (Killer == null)
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
		var ct = 1f;
		var tt = TimeText.color;
		
		Utils.Animate(1f, 0f, GOAnimationTime / 4, (v) =>
		{
			CameraScript.Instance.InvProgress += v;
			ct += v;
			tt.r = 1 - ct;
			tt.g = 1 - ct;
			tt.b = 1 - ct;
			TimeText.color = tt;

		});
//		CancelInvoke("TickUpdate");
		_ticking = false;
	}

	public void KillEverything(bool butPlayer = false)
	{
		GetAllUnits().ForEach((u) =>
		{
			if (butPlayer && u is Player) return;
			u.TakeDmg(null, 999);
		});
	}
}
