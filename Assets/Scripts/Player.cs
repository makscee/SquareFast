using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

	private void Update()
	{
		if (Input.GetButtonDown("Right"))
		{
			MoveOrAttack(1);
		}
		if (Input.GetButtonDown("Left"))
		{
			MoveOrAttack(-1);
		}
	}
}
