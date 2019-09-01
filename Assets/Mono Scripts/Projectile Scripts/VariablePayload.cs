﻿using System.Collections;
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

	private int[] upgradeLevels = null;

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
		shot.SetUpgrades(upgradeLevels);

		shot.Launch(direction, adjustedPower);

		Destroy(gameObject);
	}

	public override void SetUpgrades(int[] upgradeLevels)
	{
		this.upgradeLevels = upgradeLevels;
	}

}
