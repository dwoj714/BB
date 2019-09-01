using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Detonator))]
public abstract class ExplosiveProjectile : ProjectileController
{
	protected Detonator detonator;

	protected override void Awake()
	{
		base.Awake();
		detonator = GetComponent<Detonator>();
	}

	protected virtual void OnExplosion()
	{
		Destroy(gameObject);
	}
}