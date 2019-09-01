using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusProjectile : ProjectileController
{
	[SerializeField] private StatusEffect[] effects;
	private int effectLevel;

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
		if(effects[effectLevel] != null) effects[effectLevel].ApplyEffect(collision.collider.gameObject);
	}

	public override void SetUpgrades(int[] upgradeLevels)
	{
		base.SetUpgrades(upgradeLevels);
		effectLevel = upgradeLevels[3];
	}
}