using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PhysCircle : MonoBehaviour
{
	public static float globalDamageMultiplier = 6;

	[HideInInspector]
	public CircleCollider2D col;

	[HideInInspector]
	public Rigidbody2D rb;

	[HideInInspector]
	public Vector2 oldVelocity;

	public Dictionary<PhysCircle, float> hitRegistry = new Dictionary<PhysCircle, float>();

	[HideInInspector]
	public HealthBar hb;

	public static GameObject hitFX;
	private ParticleSystem.MainModule particleModule;

	protected Vector2 lastCollisionNormal = Vector2.zero;
	protected Vector3 lastCollisionPoint = Vector3.zero;

	protected virtual void Awake()
	{
		col = GetComponent<CircleCollider2D>();
		rb = GetComponent<Rigidbody2D>();
		hb = GetComponent<HealthBar>();
	}

	protected virtual void FixedUpdate()
	{
		//Used to save the velocity for use during a collision in the same physics update
		oldVelocity = rb.velocity;
	}

	//Returns the radius of the collider adjusted by how it's scaled
	public float AdjustedRadius
	{
		get
		{
			float greaterScale = transform.lossyScale.x > transform.lossyScale.y ? transform.lossyScale.x : transform.lossyScale.y;
			return col.radius * greaterScale;
		}
	}

	public float LocalRadius
	{
		get
		{
			float greaterScale = transform.localScale.x > transform.localScale.y ? transform.localScale.x : transform.localScale.y;
			return col.radius * greaterScale;
		}
		set
		{
			value = Mathf.Abs(value);
			transform.localScale = Vector3.up * value + Vector3.right * value;
		}
	}

	protected virtual void OnCollisionEnter2D(Collision2D hit)
	{
		lastCollisionNormal = hit.GetContact(0).normal;
		lastCollisionPoint = hit.GetContact(0).point;

		ProcessCollision(hit.collider.gameObject, hit.GetContact(0).point);
	}

	void ProcessCollision(GameObject hitObj, Vector3 point)
	{
		PhysCircle hitCircle = hitObj.GetComponent<PhysCircle>();

		//If hitCircle exists
		if (hitCircle && !hitCircle.rb.isKinematic)
		{
			//Represents the position of hitCircle relative to this physcircle
			Vector2 relativePosition = hitCircle.rb.position - rb.position;

			//The magnitude of the velocity vector projected onto a vector between both PhysCircles centers
			float VPM = oldVelocity.magnitude * Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(relativePosition, oldVelocity));

			//Add this circle and its VPM to the hit PhysCircles hit registry
			hitCircle.hitRegistry.Add(this, VPM);

			//If we have the other circle in our hit registry, use the VPM associated with it
			//to deal collision damage to both PhysCircles' health bars; whichever ones have them
			if (hitRegistry.ContainsKey(hitCircle))
			{
				float massTotal = rb.mass + hitCircle.rb.mass;

				//the VPM of the other physCircle
				float hitVPM = hitRegistry[hitCircle];

				//The objects' combined masses times their combined VPMs
				float baseDamage = Mathf.Abs(hitVPM + VPM);

				//Deal damage to any existing health bars
				if (baseDamage > 1)
				{
					if (hb)
					{
						hb.TakeDamage(globalDamageMultiplier * baseDamage * (hitCircle.rb.mass / massTotal), hitCircle.gameObject);
					}
					if (hitCircle.hb)
					{
						hitCircle.hb.TakeDamage(globalDamageMultiplier * baseDamage * (rb.mass / massTotal), gameObject);
					}

					//Give spawned particles intensity reflecting the speed of each mass
					float i1 = Mathf.Log(rb.mass, 4) * Mathf.Abs(VPM);
					float i2 = Mathf.Log(hitCircle.rb.mass, 4) * Mathf.Abs(hitVPM);
					SpawnCollisionParticles(i1 + i2);
				}
				hitRegistry.Remove(hitCircle);
			}
		}
		else //If the collision is with terrain (or any other non-PhysCircle collider)
		{

			float VPM = oldVelocity.magnitude * Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(lastCollisionNormal, oldVelocity));
			float baseDamage = Mathf.Abs(VPM) * globalDamageMultiplier;
			if (baseDamage > 1)
			{
				if (hb)
				{
					hb.TakeDamage(baseDamage, gameObject);
				}
				SpawnCollisionParticles(Mathf.Log(rb.mass, 4) * Mathf.Abs(VPM));
			}
		}
	}

	private void SpawnCollisionParticles(float intensity)
	{
		//calculate the angle to spawn the particles with
		float zAngle = Vector2.SignedAngle(Vector2.up, lastCollisionNormal);

		//spawn the particles object, keep a reference to the ParticleSystem
		particleModule = Instantiate(hitFX, lastCollisionPoint, Quaternion.Euler(0, 0, zAngle)).GetComponent<ParticleSystem>().main;
	
		particleModule.startSpeed = intensity;
	}

}
