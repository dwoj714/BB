using System;
using UnityEngine;

[RequireComponent(typeof(Detonator))]
public abstract class ExplosiveProjectile : ProjectileController
{
	protected Detonator detonator;

	[SerializeField] protected float damageBonus = 0.12f;
	[SerializeField] protected float impulseBonus = 0.15f;
	[SerializeField] protected float radiusBonus = 0.12f;

	protected override void Awake()
	{
		base.Awake();
		if(!detonator) detonator = GetComponent<Detonator>();
		detonator.ExplosionEventHandler += OnExplosion;
	}

	//Default behaviour, destroy on explosion
	protected virtual void OnExplosion(object source, EventArgs args)
	{
		Destroy(gameObject);
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
			try
			{
				detonator.maxExplosionDMG *= 1 + value[6] * damageBonus;
				detonator.minExplosionDMG *= 1 + value[6] * damageBonus;

				detonator.maxPushForce *= 1 + value[7] * impulseBonus;
				detonator.minPushForce *= 1 + value[7] * impulseBonus;

				detonator.explosionRadius *= 1 + value[8] * radiusBonus;
			}
			catch (System.IndexOutOfRangeException)
			{
				Debug.LogWarning("Too short of an upgrade list sent to ExplosiveProjectile on " + name);
			}
		}
	}

}