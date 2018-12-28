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

	[SerializeField]
	public Dictionary<PhysCircle, float> hitRegistry = new Dictionary<PhysCircle, float>();

	HealthBar hb;

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
			float greaterScale = transform.lossyScale.x > transform.lossyScale.y ? transform.lossyScale.z : transform.lossyScale.y;
			return col.radius * greaterScale;
		}
	}

	protected virtual void OnCollisionEnter2D(Collision2D hit)
	{
		//The PhysCircle we just collided with
		ProcessCollision(hit.collider.gameObject);

		/*ContactPoint2D[] points = new ContactPoint2D[6];

		int count = col.GetContacts(points);

		//Debug.Log(name + " Contact count: " + count);

		for(int i=0; i < count; i++)
		{
			if(points[i].collider != hit.collider)
			{
				//May need to pass a VPM "Override" calculated from the predicted velocity after the collision
				ProcessCollision(points[i].collider.gameObject);
				Debug.Log(gameObject.name + " Extra collision processed: " + points[i].collider.name);
			}
		}*/

	}

	void ProcessCollision(GameObject hitObj)
	{
		PhysCircle hitCircle = hitObj.GetComponent<PhysCircle>();

		//If hitCircle exists
		if (hitCircle && !hitCircle.rb.isKinematic /*&& !hitCircle.hitList.Contains(this)*/)
		{
			//Represents the position of hitCircle relative to this physcircle
			Vector2 relativePosition = hitCircle.rb.position - rb.position;

			//The magnitude of the velocity vector projected onto a vector between both PhysCircles centers
			float VPM = oldVelocity.magnitude * Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(relativePosition, oldVelocity));

			//Add this circle and its VPM to the hit PhysCircles hit registry
			hitCircle.hitRegistry.Add(this, VPM);

			//Debug.Log(name + " collision with " + hitCircle.name + ". Registry size of " + name +": " + hitRegistry.Count);

			//If we have the other circle in our hit registry, use the VPM associated with it
			//to deal collision damage to both PhysCircles' health bars; whichever ones have them
			if (hitRegistry.ContainsKey(hitCircle))
			{
				float massTotal = rb.mass + hitCircle.rb.mass;
				float collisionVPM = VPM + hitRegistry[hitCircle];

				//The objects' combined masses times their combined VPMs
				float baseDamage = Mathf.Abs(collisionVPM);

				//Deal damage to any existing health bars
				if (hb)
				{
					hb.TakeDamage(globalDamageMultiplier * baseDamage * (hitCircle.rb.mass / massTotal), hitCircle.gameObject);
				}
				if (hitCircle.hb)
				{
					hitCircle.hb.TakeDamage(globalDamageMultiplier * baseDamage * (rb.mass / massTotal), gameObject);
				}
				hitRegistry.Remove(hitCircle);
			}
		}
		else //If the collision is with terrain (or any other non-PhysCircle collider)
		{
			if (hb)
			{
				ContactPoint2D[] points = new ContactPoint2D[1]; 

				col.GetContacts(points);

				Vector2 normal = points[0].normal;
				float VPM = oldVelocity.magnitude * Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(normal, oldVelocity));

				float baseDamage = Mathf.Abs(VPM) * globalDamageMultiplier;// * rb.mass;

				if (baseDamage > 1)
					hb.TakeDamage(baseDamage, gameObject);
			}
		}
	}

	/*protected void OnCollisionExit2D(Collision2D hit)
	{
		PhysCircle hitCircle = hit.gameObject.GetComponent<PhysCircle>();


	}*/

}
