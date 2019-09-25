using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboIncrement : Buff
{
	public int comboBonus = 1;
	public override bool ApplyEffect(GameObject target)
	{
		BombController bomb = target.GetComponent<BombController>();
		if(bomb)
		{
			bomb.comboMult += comboBonus;
			bomb.detonator.incrementer.comboBonus += bomb.comboMult;
			return true;
		}
		return false;
	}
}
