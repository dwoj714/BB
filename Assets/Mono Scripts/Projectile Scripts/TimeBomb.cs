using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBomb : ExplosiveProjectile
{
	[Header("Time Bomb Behavior", order = 0)]
	[SerializeField] private int bombContactLimit = 3;
	private int contactCount = 0;

    public override void Launch(Vector2 direction, float power)
	{
		base.Launch(direction, power);
		detonator.sparked = true;
	}

	protected override void OnCollisionEnter2D(Collision2D other)
	{
		base.OnCollisionEnter2D(other);
		if(other.collider.gameObject.layer == 12)
		{
			contactCount++;
			if(contactCount == bombContactLimit)
			{
				detonator.Explode();
			}
		}
	}

}
