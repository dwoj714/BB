using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShot : AmmoType
{
	public int shotCount = 5;
	public AmmoType projectileType;
	public float minSpreadDegrees, maxSpreadDegrees = 45;

	//Launches [shotcount] projectiles in an arc [spreadDegrees] degrees wide
	public override void Launch(Vector2 direction, float power)
	{
		direction.Normalize();
		AmmoType shot;

		//Degrees of shot spreading between min and max spread degrees based on power
		float spreadDegrees = (maxSpreadDegrees - minSpreadDegrees) * power + minSpreadDegrees;

		for(int i = 0; i < shotCount; i++)
		{
			shot = Instantiate(projectileType, transform.position, Quaternion.identity);

			float shotAngle = Vector2.SignedAngle(Vector2.right, direction) + (i / (float)(shotCount - 1)* spreadDegrees) - spreadDegrees/2;
			Vector2 shotDirection = new Vector2(Mathf.Cos(shotAngle * Mathf.Deg2Rad), Mathf.Sin(shotAngle * Mathf.Deg2Rad));

			shot.launcherCollider = launcherCollider;
			shot.Launch(shotDirection, power);
		}
		Destroy(gameObject);
	}


}
