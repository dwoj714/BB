using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsar : ExplosiveProjectile
{
	[SerializeField] private int explosions = 8;

	[SerializeField] private float minFuse, maxFuse = 0.5f;

	public override void Launch(Vector2 direction, float power)
	{
		power = Mathf.Sqrt(power);

		base.Launch(direction, power);
		detonator.sparked = true;
		//detonator.autoDeleteFX = false;
		detonator.screenShake = false;

		//set the fuse to minFuse at full charge, max at lowest charge
		float fDiff = (1 - power) * (maxFuse - minFuse);
		detonator.fuse = minFuse + fDiff;

	}

	private void LateUpdate()
	{
		if (!detonator.sparked && launched) detonator.sparked = true;
	}

	protected override void OnExplosion()
	{
		explosions--;
		if(explosions == 0)
		{
			Destroy(gameObject);
		}
		else if(explosions == 1)
		{
			detonator.autoDeleteFX = true;
		}
	}
}
