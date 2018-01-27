using System.Xml.Schema;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : Unit
{
	public static Player Instance;
	public bool Autopilot;
	public static Prefab Prefab = new Prefab("Player");
	public bool GameOverInstance = false;
	public Unit Swallowed;

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
		if (Input.GetKeyDown(KeyCode.Escape))
		{
		    SceneManager.LoadScene(0);
			UnitedTint.Tint = Color.white;
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			TakeDmg(this, 999);
			return;
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			Autopilot = true;
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
		if (Autopilot)
		{
			Unit u;
			if (u = Level.Instance.Get(Position.IntX() + 1))
			{
				u.HP = 1;
				rightDown = true;
			} else if (u = Level.Instance.Get(Position.IntX() - 1))
			{
				u.HP = 1;
				leftDown = true;
			}
		}
		var dir = leftDown ? -1 : (rightDown ? 1 : 0);
		if (dir == 0) return;

		var gm = GridMarks.Instance;
		if (Menu.Instance.isActiveAndEnabled)
		{
			if (gm.FieldSize() > 2)
			{
				gm.ShiftBorder(dir);
			}
			if (CameraScript.MenuZoomout > 0)
			{
				Utils.Animate(0f, 1f, 0.2f, (v) => CameraScript.MenuZoomout -= v);
			}
		}

		MoveOrAttack(dir);
	}

	protected override bool MoveOrAttack(int relDir)
	{
		if (Swallowed != null)
		{
			AttackAnim(relDir);
			Utils.InvokeDelayed(() =>
			{
				if (Swallowed == null) return;
				if (Swallowed.TakeDmg(this, 1))
				{
					Swallowed = null;
				}
			}, AnimationWindow / 2);
			return true;
		}
		return base.MoveOrAttack(relDir);
	}

	public void HandleBoundries(bool left)
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
			}
		}
		TakeDmg(this, 9999);
	}

	public override bool TakeDmg(Unit source, int dmg = 1)
	{
		if (source != null)
		{
			Level.Instance.Killer = source.GetPrefab();
			Level.Instance.KillerHP = source.MaxHP;
		}
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
		if (!Level.GameOver && !GameOverInstance)
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
