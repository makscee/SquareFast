using System.Xml.Schema;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : Unit
{
	public static Player Instance;
	public int PushTicks = -1;
	public static bool Godmode;
	public static Prefab Prefab = new Prefab("Player");
	public bool GameOverInstance = false;
	public Unit Swallowed;
	public bool HasTutorialHitChance = true;
	public int TutorialHitChanceDir = 0;
	private bool _tutSpawned;
	public GameObject Spikes;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (Menu.Instance.isActiveAndEnabled && !Menu.Instance.CanvasHidden)
		{
			return;
		}
		bool leftDown = Input.GetButtonDown("Left"),
			rightDown = Input.GetButtonDown("Right");
		if (Input.GetKeyDown(KeyCode.R))
		{
			TakeDmg(this, 999);
			return;
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			Godmode = !Godmode;
		}
		if (Input.touchCount > 0)
		{
			for (var i = 0; i < Input.touchCount; ++i)
			{
				if (Input.GetTouch(i).phase != TouchPhase.Began) continue;
				var pos = Input.GetTouch(i).position.x / Camera.main.pixelWidth;
				if (pos > 0.5f)
					rightDown = true;
				else
					leftDown = true;
			}
		}
		if (Godmode)
		{
			Unit u;
			// ReSharper disable once AssignmentInConditionalExpression
			if (u = Level.Instance.Get(Position.IntX() + 1))
			{
				if (!(u is DownTriangleEnemy))
				{
					u.HP = 1;
					rightDown = true;
				}
				// ReSharper disable once AssignmentInConditionalExpression
			} else if (u = Level.Instance.Get(Position.IntX() - 1))
			{
				if (!(u is DownTriangleEnemy))
				{
					u.HP = 1;
					leftDown = true;
				}
			}
		}
		var dir = leftDown ? -1 : (rightDown ? 1 : 0);
		if (TutorialHitChanceDir != 0 && TutorialHitChanceDir != dir)
		{
			dir = 0;
		}
		if (dir == 0) return;

		if (Menu.Instance.isActiveAndEnabled && !Level.Tutorial)
		{
			var gm = GridMarks.Instance;
			if (gm.FieldSize() > 2)
			{
				gm.ShiftBorder(dir);
			}
		} else if (Level.Tutorial && !Level.Spawning && !_tutSpawned && !Menu.Instance.HintCanvas.activeSelf)
		{
			Debug.LogWarning("tutorial spawn");
			_tutSpawned = true;
			var e = BasicEnemy.Prefab.Instantiate();
			e.transform.position = new Vector3(-dir, 0, 0);
			var u = e.GetComponent<Unit>();
			u.HP = 1;
			UnitHint.CreateUnitText("^\nKILL", u);
			u.DieEvent += () => {
				var e1 = BasicEnemy.Prefab.Instantiate();
				e1.transform.position = new Vector3(dir * 2, 0, 0);
				var ud1 = e1.GetComponent<Unit>();
				UnitHint.CreateUnitText("^\nKILL", ud1);
				ud1.HP = 1;
				e1 = BasicEnemy.Prefab.Instantiate();
				e1.transform.position = new Vector3(-dir * 2, 0, 0);
				var ud2 = e1.GetComponent<Unit>();
				UnitHint.CreateUnitText("^\nKILL", ud2);
				ud2.HP = 1;
				ud2.DieEvent += () =>
				{
					if (ud1 != null) return;
					var e2 = BasicEnemy.Prefab.Instantiate();
					e2.transform.position = new Vector3(dir, 0, 0);
					var u2 = e2.GetComponent<Unit>();
					UnitHint.CreateUnitText("^\nKILL", u2);
					u2.HP = 1;
					u2.DieEvent += () =>
					{
						Level.Instance.StartLevel();
						GridMarks.Instance.BringBackTint(-1);
						GridMarks.Instance.BringBackTint(1);
//						GridMarks.Instance.SetBorders(0, 0);
						GridMarks.Instance.ShiftBorder(1, false);
						GridMarks.Instance.ShiftBorder(-1, false);
					};
				};
				ud1.DieEvent += () =>
				{
					if (ud2 != null) return;
					var e2 = BasicEnemy.Prefab.Instantiate();
					e2.transform.position = new Vector3(-dir, 0, 0);
					var u2 = e2.GetComponent<Unit>();
					UnitHint.CreateUnitText("^\nKILL", u2);
					u2.HP = 1;
					u2.DieEvent += () =>
					{
						Level.Instance.StartLevel();
						GridMarks.Instance.BringBackTint(-1);
						GridMarks.Instance.BringBackTint(1);
						GridMarks.Instance.SetBorders(0, 0);
					};
				};
			};
		}
		MoveOrAttack(dir);
	}

	protected override bool MoveOrAttack(int relDir)
	{
		if (Swallowed != null)
		{
			AttackAnim(relDir);
//			Utils.InvokeDelayed(() =>
//			{
//				if (Swallowed == null) return;
				if (Swallowed.TakeDmg(this, 1))
				{
					Swallowed = null;
				}
//			}, AnimationWindow / 2);
			return true;
		}
		return base.MoveOrAttack(relDir);
	}

	public void HandleBoundaries(bool left)
	{
		if (Level.GameOver && GameOverInstance)
		{
			if (left)
			{
				Level.Instance.ExitGameover();
			}
			else
			{
				SceneManager.LoadScene(0);
				Time.timeScale = 1;
			}
		}
		TakeDmg(this, 9999);
	}

	public override bool TakeDmg(Unit source, int dmg = 1)
	{
		Level.JustFinishedTutorial = Level.StartedLevel == 0;
//		if (source != null)
//		{
//			Level.Instance.Killer = source.GetPrefab();
//			Level.Instance.KillerHP = source.MaxHP;
//		}
		return base.TakeDmg(source, dmg);
	}

	public override void Die()
	{
		if (Swallowed != null)
		{
			Swallowed.TakeDmg(this, 999);
		}
		if (Menu.Instance.isActiveAndEnabled)
		{
			base.Die();
			return;
		}
		if (Level.Tutorial)
		{
			ProgressLine.Instance.Reset();
			ProgressLine.Instance.Updating = false;
			Utils.Animate(1f, 0f, 0.5f, (v) => Level.Instance.AudioSource.volume += v);
			Level.Updating = false;
			CounterScript.Instance.Updating = false;
			Utils.InvokeDelayed(() =>
			{
				Level.Instance.KillEverything(true);
				CameraScript.Instance.SwitchScene(() =>
				{
					Level.Instance.OnEnable();
					GridMarks.Instance.BringBackTint(-1);
					GridMarks.Instance.BringBackTint(1);
					GridMarks.Instance.SetBorders(0, 0);
					CounterScript.Instance.Text.text = "0.00";
				});
			}, 1f);
		} else if (!Level.GameOver && !GameOverInstance)
		{
			Level.Instance.EnterGameOver();
		}
//		if (GameOverInstance && Level.GameOver)
//		{
//			Utils.InvokeDelayed(() =>
//			{
//				Level.Instance.RespawnGOUnits();
//			}, 1f);
//		}
		base.Die();
	}
}
