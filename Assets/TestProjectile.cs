using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : ProjectileController {

	public Vector2 initialVelocity;

	public void Start()
	{
		Launch(initialVelocity, 1);
	}

	public override void Launch(Vector2 velocity, float power)
	{
		rb.isKinematic = false;
		rb.velocity = velocity.normalized;
		if (hasFixedSpeed)
		{
			fixedSpeed = velocity.magnitude;
		}
	}

	protected override void FixedUpdate()
	{
	//	base.base.FixedUpdate();	Can't do that...
	}
}
