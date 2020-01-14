using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShot : Launchable
{
	public int shotCount = 5;
	public Launchable projectileType;
	public float minSpreadDegrees, maxSpreadDegrees = 45;

	[Header("Upgrade Scaling")]
	[SerializeField] private float chokeFactor = 0.4f;

	//Launches [shotcount] projectiles in an arc [spreadDegrees] degrees wide
	public override void Launch(Vector2 direction, float power)
	{
		direction.Normalize();
		Launchable shot;

		//Degrees of shot spreading between min and max spread degrees based on power
		float spreadDegrees = (maxSpreadDegrees - minSpreadDegrees) * power + minSpreadDegrees;

		//Spawn each shot, give them the proper angles, and apply upgrades
		for(int i = 0; i < shotCount; i++)
		{
			//instantiate a launchable called shot
			shot = Instantiate(projectileType, transform.position, Quaternion.identity);

			//calculate the angle & vector to send it off with
			float shotAngle = Vector2.SignedAngle(Vector2.right, direction) + (i / (float)(shotCount - 1) * spreadDegrees) - spreadDegrees/2;
			Vector2 shotDirection = new Vector2(Mathf.Cos(shotAngle * Mathf.Deg2Rad), Mathf.Sin(shotAngle * Mathf.Deg2Rad));

			//Apply upgrades to it
			shot.UpgradeLevels = upgradeLevels;

			//this step
			shot.launcherCollider = launcherCollider;

			//off ye go
			shot.Launch(shotDirection, power);
		}
		Destroy(gameObject);
	}

	public override int[] UpgradeLevels
	{
		set
		{
			base.UpgradeLevels = value;

			shotCount += upgradeLevels[12];
			maxSpreadDegrees *= 1/(1 + upgradeLevels[13] * chokeFactor);
			minSpreadDegrees *= 1 / (1 + upgradeLevels[13] * (chokeFactor / 2));
		}
	}

	public override float LaunchSpeed(float power)
	{
		return projectileType.LaunchSpeed(power);
	}

}
