using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An ability which does an action at a given [mouse] coordinate
public abstract class PointAbility : Ability
{
	private bool armed;

	public bool Armed
	{
		get
		{
			return armed;
		}
	}

	public abstract void Activate(Vector2 position);

	public void Arm(InputManager inputManager)
	{
		inputManager.ControlledObject = "Ability";
		inputManager.Ability = this;
		armed = true;
	}

	public void Disarm(InputManager inputManager)
	{
		inputManager.ControlledObject = "Launcher";
		armed = false;
	}
}
