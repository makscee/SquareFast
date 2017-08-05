using UnityEngine;

public class Player : Unit
{
	public static Player Instance;
	public bool Autopilot;
	public static Prefab Prefab = new Prefab("Player");
	public bool GameOverInstance = false;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		bool leftDown = Input.GetButtonDown("Left"),
			rightDown = Input.GetButtonDown("Right");
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
		MoveOrAttack(dir);
	}

	public void HandleBoundries()
	{
		if (Level.GameOver && GameOverInstance)
		{
			if (transform.position.x < 0)
			{
				Level.Instance.ExitGameover();
			}
			else
			{
				return;
			}
		}
		TakeDmg(this, 9999);
	}

	public override void TakeDmg(Unit source, int dmg = 1)
	{
		if (source != null)
		{
			Level.Instance.Killer = source.GetPrefab();
			Level.Instance.KillerHP = source.MaxHP;
		}
		base.TakeDmg(source, dmg);
	}

	public override void Die()
	{
		if (!Level.GameOver && !GameOverInstance)
		{
			Level.Instance.EnterGameOver();
		}
		if (GameOverInstance && Level.GameOver)
		{
			Utils.InvokeDelayed(() =>
			{
				Level.Instance.RespawnGOUnits();
			}, 1f);
		}
		base.Die();
	}
}
