using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShot : Launchable
{
	public int shotCount = 5;
	public Launchable projectileType;
	public float minSpreadDegrees, maxSpreadDegrees = 45;

	private int[] upgradeLevels;

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
			float shotAngle = Vector2.SignedAngle(Vector2.right, direction) + (i / (float)(shotCount - 1)* spreadDegrees) - spreadDegrees/2;
			Vector2 shotDirection = new Vector2(Mathf.Cos(shotAngle * Mathf.Deg2Rad), Mathf.Sin(shotAngle * Mathf.Deg2Rad));

			//make it apply whatever upgrades
			shot.SetUpgrades(upgradeLevels);

			//this step
			shot.launcherCollider = launcherCollider;

			//off ye go
			shot.Launch(shotDirection, power);
		}
		Destroy(gameObject);
	}

	//upgradeLevels is sent to each projectile after instantiation
	public override void SetUpgrades(int[] upgradeLevels)
	{
		this.upgradeLevels = new int[upgradeLevels.Length];
		upgradeLevels.CopyTo(this.upgradeLevels, 0);
		shotCount += upgradeLevels[4];
		this.upgradeLevels[4] = 0;
	}

}
