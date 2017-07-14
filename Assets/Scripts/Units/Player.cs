using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : Unit
{
	public static Player Instance;
	public Text DelayText;

	private void Awake()
	{
		Instance = this;
	}

	private float dt = 0;
	private void Update()
	{
		dt += Time.deltaTime;
		bool leftDown = Input.GetButtonDown("Left"),
			rightDown = Input.GetButtonDown("Right");
		var dir = leftDown ? -1 : (rightDown ? 1 : 0);
		if (dir == 0) return;
		DelayText.text = Math.Round(dt / Level.TickTime / 3, 3).ToString();
		MoveOrAttack(dir);
	}

	public override bool TickUpdate()
	{
		if (Level.Ticks % 3 == 0)
		{
			dt = 0;
		}
		return base.TickUpdate();
	}

	public override void Die()
	{
		base.Die();
		CameraScript.Instance.GetComponent<SpritePainter>().Paint(new Color(0.43f, 0f, 0.01f), 1.5f, true);
		Level.Instance.Restart();
	}
}
