using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablePayload : AmmoType
{
	[Range(0, 1)]
	public float transitionPower = 0.5f;

	//Projectile Type to launch when power is below transitionPower
	public AmmoType lowPowerAmmo;

	//Projectile type to launch when power is above transitionPower
	public AmmoType highPowerAmmo;

	public override void Launch(Vector2 direction, float power)
	{
		//Debug.Log("Base Power: " + power + (power < transitionPower));
		if(power < transitionPower)
		{
			AmmoType shot = Instantiate(lowPowerAmmo, transform.position, Quaternion.identity);

			shot.launcherCollider = launcherCollider;

			//adjustedPower will have a range from 0 - 1
			float adjustedPower = power / transitionPower;
			//Debug.Log(adjustedPower);
			shot.Launch(direction, adjustedPower);
		}
		else
		{
			AmmoType shot = Instantiate(highPowerAmmo, transform.position, Quaternion.identity);

			shot.launcherCollider = launcherCollider;

			float adjustedPower = (power - transitionPower) / (1 - transitionPower);
			shot.Launch(direction, adjustedPower);
		}
		Destroy(gameObject);
	}

}
