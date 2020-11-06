 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonator : MonoBehaviour {

	public ExplosionInstance VFX;

	
	public float minPushForce, maxPushForce;
	public float minExplosionDMG, maxExplosionDMG;
	public float explosionRadius;
	public float fuse = 0.5f;
	public bool screenShake = true;

	//increase damage/knockback proportionally with BombController's mass/health modifiers (if set to 1)
	public float damageScaling = 0;
	public float knockbackScaling = 0;

	CameraController cam;

	public bool useMask = false;

	public LayerMask explosionMask;

	float fuseTimer;

	//When this is true, the fuse is shortened via deltaTime
	public bool sparked;
	[HideInInspector]
	public bool sparkedLastFrame;

	[HideInInspector]
	PhysCircle circle;

	private BombController attachedBomb;

	//private ExplosionInstance fx;

	[HideInInspector] public bool autoDeleteFX = true;

	public delegate void Explosion();
	public event Explosion ExplosionEventHandler;

	public float FusePercent
	{
		get
		{
			return fuseTimer / fuse;
		}
	}

	void Awake()
	{
		fuseTimer = fuse;
		circle = GetComponent<PhysCircle>();
		cam = Camera.main.GetComponent<CameraController>();
		attachedBomb = GetComponent<BombController>();

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
		if (fuseTimer > fuse) fuseTimer = fuse;
	}

	//Not sure if this actually needs to be in lateUpdate but it can't hurt can it?
	private void LateUpdate()
	{
		sparkedLastFrame = sparked;
	}

	public void Explode()
	{
		//Apply camera shale
		if(screenShake) cam.shakeIntensity += Mathf.Log(Mathf.Sqrt(explosionRadius), 50)/2;

		//When exploding, check for colliders within the blast radius
		Collider2D[] results;
		if (useMask)
			results = Physics2D.OverlapCircleAll(circle.transform.position, explosionRadius, explosionMask);
		else
			results = Physics2D.OverlapCircleAll(circle.transform.position, explosionRadius);
		
		//How to affect things hit by the explosion
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
					float adjustedRadius = explosionRadius - circle.AdjustedRadius;

					//Add forces to colliders with rigidbodies
					if (hitRb)
					{
						float pushScale = 1 + knockbackScaling * (BombController.massMod - 1);
						pushForce = ((adjustedRadius - dst) / adjustedRadius * (forceRange) + minPushForce) * pushScale;
						hitRb.AddForce(cd.normal * pushForce * (cd.distance < 0 ? 1 : -1), ForceMode2D.Impulse);
					}

					//if this detonator is attached to a bomb, process combo stuff for bombs hit by the explosion
					if (attachedBomb)
					{
						BombController hitBomb = hitCol.GetComponent<BombController>();
						if (hitBomb)
						{
							hitBomb.PrimeCombo(attachedBomb, 1);
						}
					}

					//deal damage to colliders attached to HealthBars
					if (hitHb)
					{
						float dmgScale = 1 + damageScaling * (BombController.healthMod - 1);
						damage = ((adjustedRadius - dst) / adjustedRadius * (dmgRange) + minExplosionDMG) * dmgScale;
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
		SendMessage("OnExplosion", SendMessageOptions.DontRequireReceiver);
		ExplosionEventHandler?.Invoke();
	}

	void OnDrawGizmos()
	{
		if (sparked)
		{
			Gizmos.DrawWireSphere(transform.position + Vector3.forward * 200, explosionRadius);
		}
	}
}
