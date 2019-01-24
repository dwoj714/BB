using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour {

	[SerializeField]
	private List<Ability> abilities;

	[SerializeField]
	private MouseInputManager inputManager;

	public void UseAbility(int id)
	{
		Debug.Log("UseAbility(" + id + ")");
		if (abilities[id] is PointAbility)
		{
			PointAbility abl = (PointAbility)abilities[id];
			abl.Arm(inputManager);
		}
	}

	public List<Ability> Abilities
	{
		get
		{
			return abilities;
		}

		set
		{
			abilities = value;
		}
	}

	public MouseInputManager InputManager
	{
		get
		{
			return inputManager;
		}

		set
		{
			inputManager = value;
		}
	}
}
