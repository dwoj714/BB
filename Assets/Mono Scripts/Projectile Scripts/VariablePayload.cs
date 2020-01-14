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
		Launchable shot;
		float adjustedPower;

		//instantiate shot to the proper object, give it the correct charge amount
		if(power < transitionPower)
		{
			shot = Instantiate(lowPowerAmmo, transform.position, Quaternion.identity);
			adjustedPower = power / transitionPower;
		}
		else
		{
			shot = Instantiate(highPowerAmmo, transform.position, Quaternion.identity);
			adjustedPower = (power - transitionPower) / (1 - transitionPower);
		}

		shot.launcherCollider = launcherCollider;

		System.Type t = shot.GetType();

		Debug.Log("VariablePayload: Shot type = " + t);	

		shot.UpgradeLevels = upgradeLevels;

		shot.Launch(direction, adjustedPower);

		Destroy(gameObject);
	}

	public override float LaunchSpeed(float power)
	{
		if (power < transitionPower)
		{
			float adjustedPower = power / transitionPower;
			return lowPowerAmmo.LaunchSpeed(adjustedPower);
		}
		else
		{
			float adjustedPower = (power - transitionPower) / (1 - transitionPower);
			return highPowerAmmo.LaunchSpeed(adjustedPower);
		}
	}

}
