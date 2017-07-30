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
			if (Level.Instance.Get(Position.IntX() + 1))
			{
				rightDown = true;
			} else if (Level.Instance.Get(Position.IntX() - 1))
			{
				leftDown = true;
			}
		}
		var dir = leftDown ? -1 : (rightDown ? 1 : 0);
		if (dir == 0) return;
		MoveOrAttack(dir);
	}

	public void HandleBoundries()
	{
		if (Level.GameOver)
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
		TakeDmgAnim(0);
	}

	public override void TakeDmg(Unit source, int dmg = 1)
	{
		Level.Instance.Killer = source.GetPrefab();
		Debug.Log(Level.Instance.Killer);
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
				Level.Instance.GetAllUnits().ForEach((u) =>
				{
					u.TakeDmg(null, 999);
					u.TakeDmgAnim(0);
				});
				var p = Prefab.Instantiate();
				p.GetComponent<Player>().GameOverInstance = true;
				if (Level.Instance.Killer != null)
				{
					var go = Level.Instance.Killer.Instantiate();
					go.transform.position = new Vector3(5, 0, 0);
					go.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
				}
			}, 1f);
		}
		base.Die();
	}
}
