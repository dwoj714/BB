using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusProjectile : ProjectileController
{
	public StatusEffect effect;

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
		effect.ApplyEffect(collision.collider.gameObject);
	}
}