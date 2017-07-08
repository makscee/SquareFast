using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Player : Unit
{
	public static Player Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (_dead)
		{
			Die();
			return;
		}
		bool leftDown = Input.GetButtonDown("Left"),
			rightDown = Input.GetButtonDown("Right");
		var dir = leftDown ? -1 : (rightDown ? 1 : 0);
		if (dir == 0) return;
		MoveOrAttack(dir);
	}

	private bool _dead = false;
	public override void Die()
	{
		if (!_dead)
		{
			_dead = true;
			Level.Instance.Clear(Position.IntX());
		}
		if (RunningAnimations != 0) return;
		base.Die();
		CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.43f, 0f, 0.01f), 1.5f, true);
		Level.Instance.Restart();
	}
}
