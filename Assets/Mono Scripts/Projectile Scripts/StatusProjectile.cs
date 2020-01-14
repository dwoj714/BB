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

	public override int[] UpgradeLevels
	{
		get
		{
			return upgradeLevels;
		}
		set
		{
			base.UpgradeLevels = value;
			effectLevel = upgradeLevels[10];
		}
	}
}