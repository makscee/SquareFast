﻿using System.Xml.Schema;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
		if (Input.GetKeyDown(KeyCode.E))
		{
			AttackAnim(1);
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
		MoveOrAttack(dir);
	}

	public void HandleBoundries(bool left)
	{
		var menu = Level.Instance as MenuLevel;
		if (menu != null)
		{
			if (!left)
			{
				menu.NextItem();
				return;
			}
			else
			{
				menu.Confirm();
			}
		}
		else if (Level.GameOver && GameOverInstance)
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
		if (Level.Instance is MenuLevel)
		{
			base.Die();
			return;
		}
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
