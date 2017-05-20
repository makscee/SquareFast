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
		if (Input.GetButtonDown("Right"))
		{
			MoveOrAttack(1);
			Level.Instance.TickUpdate();
		}
		if (Input.GetButtonDown("Left"))
		{
			MoveOrAttack(-1);
			Level.Instance.TickUpdate();
		}
	}
}
