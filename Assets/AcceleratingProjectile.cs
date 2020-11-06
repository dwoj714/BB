using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratingProjectile : ProjectileController
{
	[Header("Accelerator-Specific")]
	[SerializeField] private float maxTopSpeed = 35f;
	[SerializeField] private float minTopSpeed = 25f;
	[SerializeField] private float minAccelRate = 2;
	[SerializeField] private float maxAccelRate = 6;

	private float topSpeed;
	private float accelRate;

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (launched && fixedSpeed < topSpeed)
		{
			fixedSpeed += accelRate * Time.deltaTime;
			if (fixedSpeed > topSpeed)
			{
				fixedSpeed = topSpeed;
			}
		}
	}

	public override void Launch(Vector2 direction, float power)
	{
		base.Launch(direction, power);

		fixedSpeed = LaunchSpeed(power);
		topSpeed = (maxTopSpeed - minTopSpeed) * power + minTopSpeed;
		accelRate = (maxAccelRate - minAccelRate) * power + minAccelRate;
	}

	protected override void OnCollisionEnter2D(Collision2D hit)
	{
		base.OnCollisionEnter2D(hit);

		//destroy the projectile on collision after reaching top speed;
		if (fixedSpeed >= topSpeed)
		{
			Destroy(gameObject);
		}
	}
}
