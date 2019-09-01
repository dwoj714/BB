using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/DoT")]
public class DamageOverTime : StatusEffect
{
	private const bool hasTickEffect = true;
	private static string effectType = "DoT";

	private BombController targetBomb;

	public float DPS;
	public ParticleSystem particleFX;

	public override string EffectType
	{
		get
		{
			return effectType;
		}
	}

	public override bool HasTickEffect
	{
		get
		{
			return hasTickEffect;
		}
	}

	//Get the healthbar of the gameobject given
	public override bool ApplyEffect(GameObject target)
	{
		targetBomb = target.GetComponent<BombController>();

		//If a BombController exists on the target, make a DamageOverTime copy with a reference to it,
		//and register it with the effect manager. Then return true. Return false otherwise.
		if(targetBomb)
		{
			DamageOverTime copy = (DamageOverTime)CreateInstance("DamageOverTime");
			copy.targetBomb = targetBomb;
			copy.duration = duration;
			copy.DPS = DPS;

			if(RegisterEffect(copy, target))
			{
				copy.particleFX = Instantiate(particleFX, target.transform);
				copy.particleFX.transform.localPosition = Vector3.zero;
				ParticleSystem.ShapeModule module = copy.particleFX.shape;
				module.radius = targetBomb.AdjustedRadius;
			}

			return true;
		}
		else return false;
	}

	//Apply DPS every tick
	public override void TickEffect(float delta)
	{
		targetBomb.hb.TakeDamage(DPS * delta, null);
	}

	//This effect doesn't make changes that need to be reverted, so do nothing here
	public override void RemoveEffect(GameObject target)
	{
		particleFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

}
