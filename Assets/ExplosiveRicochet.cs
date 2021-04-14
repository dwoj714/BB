using System;
using UnityEngine;

public class ExplosiveRicochet : ExplosiveProjectile
{
    [SerializeField] private int explosions = 5;

	protected override void Awake()
	{
		detonator = GetComponentInChildren<Detonator>();
		base.Awake();
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		detonator.transform.position = collision.GetContact(0).point;
		detonator.Explode();
	}

	protected override void OnExplosion(object source, EventArgs args)
	{
		explosions--;
		if (explosions <= 0)
			Destroy(gameObject);
	}

}
