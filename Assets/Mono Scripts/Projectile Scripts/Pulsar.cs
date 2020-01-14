using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsar : ExplosiveProjectile
{
	[Header("Pulsar-Specific")]
	[SerializeField] private int explosions = 8;
	[SerializeField] private float minFuse, maxFuse = 0.5f;

	[Header("Pulsar Upgrades")]
	[SerializeField] private float explosionCountBonus = 0.2f;

	private int baseExplosions;

	protected override void Awake()
	{
		base.Awake();
		baseExplosions = explosions;
	}

	public override void Launch(Vector2 direction, float power)
	{
		power = Mathf.Sqrt(power);

		base.Launch(direction, power);
		detonator.sparked = true;
		detonator.screenShake = false;

		//set the fuse to minFuse at full charge, max at lowest charge
		float fDiff = (1 - power) * (maxFuse - minFuse);
		detonator.fuse = minFuse + fDiff;

		float fuse = detonator.fuse;

		//alter the fuse so explosion density is accurate
		detonator.fuse /= ((float)explosions) / ((float)baseExplosions);

		Debug.Log(fuse + " -- Fuse /= " + ((float)explosions) / ((float)baseExplosions));
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

	public override int[] UpgradeLevels
	{
		get
		{
			return upgradeLevels;
		}
		set
		{
			base.UpgradeLevels = value;

			explosions = Mathf.FloorToInt(baseExplosions * (1 + explosionCountBonus * upgradeLevels[11]));
		}
	}

}
