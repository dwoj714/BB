using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : AmmoType
{
	//The launcher uses a percentage to determine actual speed. 
	//0% results in minSpeed, 100% results in maxSpeed
	public float maxSpeed = 5;
	public float minSpeed = 0;

	public int bounces = 3;
	private int bounceCount = 0;

	public bool hasFixedSpeed = false;

	//Whether or not the projectile has left the launcher's circle collider
	private bool escaped;

	protected float fixedSpeed;

	protected override void Awake()
	{
		base.Awake();
		gameObject.layer = 10;
	}

	public override void Launch(Vector2 direction, float power)
	{
		float vDiff = maxSpeed - minSpeed;
		rb.isKinematic = false;
		rb.velocity = direction.normalized * (vDiff * power + minSpeed);
		if (hasFixedSpeed)
		{
			fixedSpeed = Mathf.Abs(vDiff * power + minSpeed);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (hasFixedSpeed)
		{
			rb.velocity = rb.velocity.normalized * fixedSpeed;
		}

		if (!escaped && col.Distance(launcherCollider).distance > 0)
		{
			escaped = true;
			gameObject.layer = 9;
		}

	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
		bounceCount++;
		if (bounceCount > bounces)
			Destroy(gameObject);
	}
}
