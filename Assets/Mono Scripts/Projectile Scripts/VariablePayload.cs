using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablePayload : Launchable
{
	[Range(0, 1)]
	public float transitionPower = 0.5f;

	//Projectile Type to launch when power is below transitionPower
	public Launchable lowPowerAmmo;

	//Projectile type to launch when power is above transitionPower
	public Launchable highPowerAmmo;

	public override void Launch(Vector2 direction, float power)
	{
		//Debug.Log("Base Power: " + power + (power < transitionPower));
		if(power < transitionPower)
		{
			Launchable shot = Instantiate(lowPowerAmmo, transform.position, Quaternion.identity);

			shot.launcherCollider = launcherCollider;

			//adjustedPower will have a range from 0 - 1
			float adjustedPower = power / transitionPower;
			//Debug.Log(adjustedPower);
			shot.Launch(direction, adjustedPower);
		}
		else
		{
			Launchable shot = Instantiate(highPowerAmmo, transform.position, Quaternion.identity);

			shot.launcherCollider = launcherCollider;

			float adjustedPower = (power - transitionPower) / (1 - transitionPower);
			shot.Launch(direction, adjustedPower);
		}
		Destroy(gameObject);
	}

}
