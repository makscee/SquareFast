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
	public static bool Spawning = false;
	public static bool NextLevelStart = false;
	public static bool IsFirstStart = true;
	public static int CurrentLevel = 0;
	public static int StartedLevel = 0;
	private static readonly List<Unit> Enemies = new List<Unit>();
	public int StartTicks = -1;
	public const int LevelsAmount = 7; 
	private LevelSpawner _levelSpawner;
	public AudioSource AudioSource;
	public Text TimeText;
	public AudioClip L1, L2, L3, Over;
	public Text BT;
	public Text ControlsText, BestTimeText;
	public float MusicStart, MusicDelay, BeatOffset, LevelBridge;
	public static bool Tutorial;

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
		AudioSource = GetComponent<AudioSource>();
	}

	public void OnEnable()
	{
		Spawning = false;
		Updating = true;
		GameOver = false;
		_started = false;
		Debug.Log(PlayerData.Instance.Scores[CurrentLevel]);
		if (PlayerData.Instance.Scores[CurrentLevel] != "0")
		{
			BT.gameObject.SetActive(true);
			BestTimeText.gameObject.SetActive(true);
			BestTimeText.text = PlayerData.Instance.Scores[CurrentLevel];
		}
		else
		{
			BT.gameObject.SetActive(false);
			BestTimeText.gameObject.SetActive(false);
		}
		Enemies.Clear();
		TickAction = () => { };
		GameOverAction += () =>
		{
			if (KillerUnit != null)
			{
				UnitHint.CreateUnitText("^\nAVENGE", KillerUnit);
			}
		};
		_levelSpawner = new LevelSpawner(CurrentLevel);
		_grid = new Grid(30);
		if (!NextLevelStart)
		{
			Player.Prefab.Instantiate();
			StartedLevel = CurrentLevel;
		}
		else
		{
			InitPos(Player.Instance);
		}
		
		var offset = LevelSpawner.Distance;
		Action a = () =>
		{
		};
		GridMarks.Instance.Set("", "", -1, 1, -offset, offset, a, a, true, true);
		GridMarks.Instance.DisplayBorders(false);
		GridMarks.Instance.RemoveTint(0);
		GridMarks.Instance.RemoveTint(-1);
		GridMarks.Instance.RemoveTint(1);
		if (Tutorial && ControlsText.isActiveAndEnabled)
		{
			var c = ControlsText.color;
			c.a = 1;
			var ut = ControlsText.GetComponent<UnitedTint>();
			ut.Color = c;
		}
		else
		{
			StartLevel();
		}
	}

	public void StartLevel()
	{
		Spawning = true;
		var delay = NextLevelStart ?  0 : MusicDelay;

		_ticking = false;
		Utils.InvokeDelayed(() => _ticking = true, delay);
		
		Ticks = StartTicks;
		if (Tutorial && ControlsText.isActiveAndEnabled)
		{
			var ut = ControlsText.GetComponent<UnitedTint>();
			var c = ut.Color;
			c = new Color(c.r, c.g, c.b, 0);
			Utils.Animate(1f, 0f, 1f, (v) =>
			{
				c.a = v;
				ut.Color = c;
			}, null, true, 1f);
			Utils.InvokeDelayed(() => ControlsText.gameObject.SetActive(false), 2f);
		}
		
		if (NextLevelStart) return;
		AudioSource.volume = 0f;
		Utils.Animate(0f, 1f, 0.9f, (v) => AudioSource.volume += v);
		AudioSource.time = MusicStart;
		AudioSource.Play();
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


	private bool _started;
	public Action StartAction, TickActionPerm = () => { }, TickAction = () => { }, GameOverStartAction = () => { }, GameOverAction = () => { }, ExitGameOverAction = () => { };

	private float _sampleTime = 0f;
	private bool _ticking = false;
	private void Update()
	{
		if (!_ticking) return;
		var newST= 1.0f *AudioSource.timeSamples / 44100 + BeatOffset;
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
		UnitedTint.TickUpdate();
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
		Enemies.Add(unit);
		_grid.Set(unit.Position.IntX(), unit);
	}

	public List<Unit> GetAllEnemies()
	{
		return Enemies.FindAll(unit => unit != null);
	}

	private const float GOAnimationTime = 1f;
	public void EnterGameOver()
	{
		GameOverStartAction();
		var score = TimeText.text;
		if (string.IsNullOrEmpty(PlayerData.Instance.Scores[StartedLevel]) || float.Parse(score) > float.Parse(PlayerData.Instance.Scores[StartedLevel]))
		{
			PlayerData.Instance.Scores[StartedLevel] = score;
			Saves.Save();
			WebUtils.SendScore(StartedLevel);
		}
		Updating = false;
		GameOver = true;
		
		Utils.Animate(1f, 0f, 0.5f, (v) => AudioSource.volume += v);
		float[] starts = {0f, 13f, 43f, 66f, 90f};
		Utils.InvokeDelayed(() =>
		{
			AudioSource.clip = Over;
			AudioSource.time = starts[UnityEngine.Random.Range(0, starts.Length)];
			AudioSource.Play();
			Utils.Animate(0f, 0.5f, 2f, (v) => AudioSource.volume += v);
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
				CurrentLevel = StartedLevel;
				Utils.InvokeDelayed(OnEnable, GOAnimationTime / 4);
			}, () =>
			{
				CameraScript.Instance.SwitchScene(() =>
				{
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
				BT.color = tt;
				BestTimeText.color = tt;
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
		Enemies.Clear();
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
			BT.color = tt;
			BestTimeText.color = tt;

		});
//		CancelInvoke("TickUpdate");
		_ticking = false;
	}

	public void KillEverything(bool butPlayer = false)
	{
		GetAllEnemies().ForEach((u) =>
		{
			if (u != null) u.TakeDmg(null, 999);
		});
		if (!butPlayer && Player.Instance != null) Player.Instance.TakeDmg(null, 999);
	}
}
