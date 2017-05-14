using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

	private void FixedUpdate()
	{
		if (Input.GetButtonDown("Right"))
		{
			Level.Instance.Move(1, this);
		}
		if (Input.GetButtonDown("Left"))
		{
			Level.Instance.Move(-1, this);
		}
	}
}
