using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitslugProjectile : ExplosiveProjectile
{

	public bool explosionEnabled = false;
	public int collisions = 3;
	private int collisionCount = 0;

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);

		collisionCount++;
		lastCollisionNormal = collision.GetContact(0).normal;
		if (collisionCount >= collisions)
		{
			if (explosionEnabled)
			{
				deathFX = null;
				detonator.Explode();
			}
			Destroy(gameObject);
		}
	}

	public override void SetUpgrades(int[] upgradeLevels)
	{
		base.SetUpgrades(upgradeLevels);
		explosionEnabled = upgradeLevels[3] != 0;
		collisions += upgradeLevels[4];
	}

}
