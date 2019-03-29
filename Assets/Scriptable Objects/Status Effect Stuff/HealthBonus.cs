using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Health Bonus")]
public class HealthBonus : Buff
{
	public VFXController pulseEffect;
	public float healAmount = 25;

	public override bool ApplyEffect(GameObject target)
	{
		HealthBar hb = target.GetComponent<HealthBar>();

		if (hb)
		{
			hb.Heal(healAmount);
			return true;
		}
		else return false;

	}
}
