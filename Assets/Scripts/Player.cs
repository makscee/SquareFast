using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Player : Unit
{
	public static Player Instance;
	private SpecialsManager _specialsManager = new SpecialsManager();

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		bool left = Input.GetButtonDown("Left"), right = Input.GetButtonDown("Right");
		if (left && Input.GetButton("Right") || right && Input.GetButtonDown("Left"))
		{
			_specialsManager.Start();
			return;
		}
		var dir = right ? 1 : (left ? -1 : 0);
		if (dir == 0) return;
		if (_specialsManager.IsStarted())
		{
			var ability = _specialsManager.Add(dir);
			if (ability != null)
			{
				ability.Use();
				_specialsManager.Reset();
			}
			return;
		}
		MoveOrAttack(dir);
		Level.Instance.TickUpdate();
	}
}
