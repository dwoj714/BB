using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitslugProjectile : ExplosiveProjectile
{

	[SerializeField] private bool explosionEnabled = false;
	[SerializeField] private bool multiBounce = true;
	public int collisions = 3;
	private int collisionCount = 0;

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);

		collisionCount++;
		lastCollisionNormal = collision.GetContact(0).normal;

		if (explosionEnabled)
		{
			detonator.Explode();
		}

		if (collisionCount >= collisions)
		{
			//don't spawn particles if there's an explosion on death
			if (explosionEnabled)
			{
				deathFX = null;
			}
			Destroy(gameObject);
		}
	}

	public override int[] UpgradeLevels
	{
		get
		{
			return upgradeLevels;
		}
		set
		{
			base.UpgradeLevels = value;

			explosionEnabled = upgradeLevels[11] != 0;

			if (multiBounce )collisions += upgradeLevels[12];
		}
	}
}
