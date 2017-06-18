using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Player : Unit
{
	public static Player Instance;
	private readonly SpecialsManager _specialsManager = new SpecialsManager();

	private void Awake()
	{
		Instance = this;
	}

	private bool _dropUpLeft = false;
	private bool _dropUpRight = false;
	private void Update()
	{
		bool leftUp = Input.GetButtonUp("Left"), 
			rightUp = Input.GetButtonUp("Right"),
			leftDown = Input.GetButtonDown("Left"), 
			rightDown = Input.GetButtonDown("Right"),
			leftPressed = Input.GetButton("Left"), 
			rightPressed = Input.GetButton("Right");
		var dir = leftDown ? -1 : (rightDown ? 1 : 0);
//		if (rightUp && _dropUpRight)
//		{
//			rightUp = _dropUpRight = false;
//		}
//		if (leftUp && _dropUpLeft)
//		{
//			leftUp = _dropUpLeft = false;
//		}
//		if (leftDown && rightPressed || rightDown && leftPressed)
//		{
//			_dropUpLeft = true;
//			_dropUpRight = true;
//			_specialsManager.Start();
//			return;
//		}
//		var dir = rightUp ? 1 : (leftUp ? -1 : 0);
//		if (_specialsManager.IsStarted())
//		{
//			if (dir == 0) return;
//			var ability = _specialsManager.Add(dir);
//			if (ability == null) return;
//			ability.Use();
//			_specialsManager.Reset();
//			return;
//		}
		if (dir == 0) return;
		MoveOrAttack(dir);
	}
}
