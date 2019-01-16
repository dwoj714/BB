using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/RemoveGravity")]
public class RemoveGravity : StatusEffect
{
	BombController targetBomb;
	float storedGravity;

	private static string effectType = "Remove Gravity";

	public override string EffectType
	{
		get
		{
			return effectType;
		}
	}

	//Copy constructor for passing a copy to the effect manager
	public RemoveGravity(BombController target, float gravity)
	{
		targetBomb = target;
		storedGravity = gravity;
	}

	//Sends a copy of this effect to the effect manager which 
	/*protected override bool RegisterEffect(StatusEffect copy, GameObject target)
	{
		base.RegisterEffect(copy, target);
		
	}*/

	public override bool ApplyEffect(GameObject target)
	{
		//Another RemoveGravity instance to pass to the effect manager
		RemoveGravity copy = (RemoveGravity)ScriptableObject.CreateInstance("RemoveGravity");

		targetBomb = target.GetComponent<BombController>();

		//Set the BombController's (not RigidBody2D) gravity to 0, and save what the old gravity was
		if (targetBomb != null)
		{
			copy.targetBomb = targetBomb;
			copy.storedGravity = targetBomb.gravity;
			copy.duration = duration;
			
			//If the effect is not present on the target (RegisterEffect returns false), actually apply the effect to the bombcontroller
			if(RegisterEffect(copy, target))
			{
				targetBomb.gravity = 0;
			}

			return true;
		}
		//Return false if the given object doesn't have a BombController
		else return false;
	}

	//Revert the gravity to what it was
	public override void RemoveEffect(GameObject target)
	{
		BombController controller = target.GetComponent<BombController>();

		Debug.Log("Restoring " + target.name + " Gravity (" + controller.gravity +") to " + storedGravity);

		controller.gravity = storedGravity;
	}
}
