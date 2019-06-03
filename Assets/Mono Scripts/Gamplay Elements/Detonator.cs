 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonator : MonoBehaviour {

	public GameObject VFX;

	[HideInInspector]
	public ComboIncrement incrementer;

	private ExplosionParameters defaultParameters;

	public float minPushForce, maxPushForce;
	public float minExplosionDMG, maxExplosionDMG;
	public float explosionRadius;
	public float fuse = .5f;

	public bool useMask = false;

	public LayerMask explosionMask;

	float fuseTimer;

	public struct ExplosionParameters
	{
		public float minPush, maxPush, minDMG, maxDMG, radius;

		public ExplosionParameters(float rad, float minP, float maxP, float minD, float maxD)
		{
			minPush = minP;
			maxPush = maxP;
			minDMG = minD;
			maxDMG = maxD;
			radius = rad;
		}
	}

	//When this is true, the fuse is shortened via deltaTime
	public bool sparked;
	[HideInInspector]
	public bool sparkedLastFrame;

	[HideInInspector]
	PhysCircle circle;

	void Awake()
	{
		fuseTimer = fuse;
		circle = GetComponent<PhysCircle>();
		explosionMask = LayerMask.GetMask("Everything");
		incrementer = ScriptableObject.CreateInstance<ComboIncrement>();

		defaultParameters = new ExplosionParameters(explosionRadius, minPushForce, maxPushForce, minExplosionDMG, maxExplosionDMG);
	}

	void Update()
	{
		if (sparked)
		{
			fuseTimer -= Time.deltaTime;
			
			if(fuseTimer <= 0)
			{
				Explode();
				sparked = false;
				fuseTimer = fuse;
			}
		}
	}

	//Not sure if this actually needs to be in lateUpdate but it can't hurt can it?
	private void LateUpdate()
	{
		sparkedLastFrame = sparked;
	}

	public void Explode()
	{
		//When exploding, check for colliders within the blast radius
		Collider2D[] results;
		if (useMask)
			results = Physics2D.OverlapCircleAll(circle.transform.position, explosionRadius, explosionMask);
		else
			results = Physics2D.OverlapCircleAll(circle.transform.position, explosionRadius);

		if (results.Length > 0)
		{
			//Declare a bunch of things to hold references to things to be affected by the explosion  
			Rigidbody2D hitRb;
			HealthBar hitHb;

			float pushForce;
			float damage;

			foreach (Collider2D hitCol in results)
			{
				//set hitRb to the rigidbody of the current collider, if it exists
				hitRb = hitCol.GetComponent<Rigidbody2D>();

				//set hitHB to the HealthBar of the current collider, if it exists
				hitHb = hitCol.GetComponent<HealthBar>();

				//The ranges of force/damage that can be applied to explosion targets, max at the edge of the collider, min at the edge of the explosion
				float forceRange = maxPushForce - minPushForce;
				float dmgRange = maxExplosionDMG - minExplosionDMG;

				if (hitCol != circle.col)
				{
					ColliderDistance2D cd = circle.col.Distance(hitCol);

					//Clamp the collider distance to positive numbers to avoid issues related to negative collider distance
					float dst = Mathf.Clamp(cd.distance, 0, Mathf.Infinity);

					//Tbh, don't remember what exact purpose this had for explosion force/damage calculations
					float adjustedRadius = explosionRadius - circle.AdjustedRadius;

					//Add forces to colliders with rigidbodies
					if (hitRb)
					{
						pushForce = (adjustedRadius - dst) / adjustedRadius * (forceRange) + minPushForce;
						hitRb.AddForce(cd.normal * pushForce * (cd.distance < 0 ? 1 : -1), ForceMode2D.Impulse);
					}

					//deal damage to colliders attached to HealthBars
					if (hitHb)
					{
						damage = (adjustedRadius - dst) / adjustedRadius * (dmgRange) + minExplosionDMG;
						if(hitHb.Health >= 1 && damage > hitHb.Health)
						{
							incrementer.ApplyEffect(hitHb.gameObject);
						}
						hitHb.TakeDamage(damage, gameObject);
					}
				}
			}
		}
		
		if (VFX)
		{
			ExplosionInstance fx = Instantiate(VFX, transform.position, Quaternion.identity).GetComponent<ExplosionInstance>();
			if (fx)
			{
				fx.SetRadius(explosionRadius);
				fx.Detonate();
			}
		}
		SendMessage("OnExplosion");
	}

	/*public void ExplosionOverride(float radius, float minPush, float maxPush, float minDMG, float maxDMG)
	{
		ExplosionParameters oldParams = new ExplosionParameters(explosionRadius, minPushForce, maxPushForce, minExplosionDMG, maxExplosionDMG);
	}*/

	void OnDrawGizmos()
	{
		if (sparked)
		{
			Gizmos.DrawWireSphere(transform.position + Vector3.forward * 200, explosionRadius);
		}
	}
}
