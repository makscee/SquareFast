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
		bool leftDown = Input.GetButtonDown("Left"),
			rightDown = Input.GetButtonDown("Right");
		var dir = leftDown ? -1 : (rightDown ? 1 : 0);
		if (dir == 0) return;
		MoveOrAttack(dir);
	}

	public override void Die()
	{
		base.Die();
		CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.43f, 0f, 0.01f), 2f, true);
		Level.Instance.Restart();
	}
}
