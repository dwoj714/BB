using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboIncrement : StatusEffect
{

	BombController bomb;

	public override string EffectType
	{
		get
		{
			return "Combo Bonus";
		}
	}

	public int comboBonus = 1;
	private int startingVal = 1;

	public override bool ApplyEffect(GameObject target)
	{
		duration = 1;

		bomb = target.GetComponent<BombController>();
		if(bomb)
		{
			bomb.comboMult += comboBonus;
			//bomb.detonator.incrementer.comboBonus += comboBonus;
			startingVal = bomb.comboMult;
			effectManager.RegisterEffect(this, target);
			return true;
		}
		return false;
	}

	//tick doesn't do anything
	public override void TickEffect(float delta){}

	public override void RemoveEffect(GameObject target)
	{
		if (bomb)
		{
			Debug.Log(bomb.name + " Removing effect");
			if (bomb.hb.Health > 0)
			{
				bomb.comboMult = startingVal;
			}
		}
	}



}
