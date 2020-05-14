using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableBomb : BombController
{
	[Header("Mass Range")]
	[SerializeField] private float maxMass = 25;
	[SerializeField] private float minMass = 5;
	[Header("Health Range")]
	[SerializeField] private float maxHealth = 150;
	[SerializeField] private float minHealth = 50;
	[Header("Size Range")]
	[SerializeField] private float maxRadius = 2.5f;
	[SerializeField] private float minRadius = 0.5f;
	[Header("Score Value Range")]
	[SerializeField] private int maxScore = 30;
	[SerializeField] private int minScore = 5;
	[Header("Detonator Ranges")]
	[SerializeField] private float maxOuterDamage = 20;
	[SerializeField] private float minOuterDamage = 5;
	[SerializeField] private float maxInnerDamage = 80;
	[SerializeField] private float minInnerDamage = 40;
	[SerializeField] private float maxOuterForce = 15;
	[SerializeField] private float minOuterForce = 0;
	[SerializeField] private float maxInnerForce = 40;
	[SerializeField] private float minInnerForce = 20;
	[SerializeField] private float maxExplosionRadius = 2;
	[SerializeField] private float minExplosionRadius = 4;
	[SerializeField] private float maxFuse = 1.2f;
	[SerializeField] private float minFuse = 0.2f;

	public void SetStats(float value)
	{
		rb.mass = minMass + (maxMass - minMass) * value;
		hb.maxHealth = minHealth + (maxHealth - minHealth) * value;
		LocalRadius = minRadius + (maxRadius - minRadius) * value;
		pointValue = Mathf.FloorToInt(minScore + (1 + maxScore - minScore) * value);

		detonator.maxExplosionDMG = minInnerDamage + (maxInnerDamage - minInnerDamage) * value;
		detonator.minExplosionDMG = minOuterDamage + (maxOuterDamage - minOuterDamage) * value;
		detonator.maxPushForce = minInnerForce + (maxInnerForce - minInnerForce) * value;
		detonator.minPushForce = minOuterForce + (maxOuterForce - minOuterForce) * value;
		detonator.explosionRadius = minExplosionRadius + (maxExplosionRadius - minExplosionRadius) * value;
		detonator.fuse = minFuse + (maxFuse - minFuse) * value;

		ApplyScaling();
	}
}
