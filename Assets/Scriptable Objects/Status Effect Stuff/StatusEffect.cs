using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : Buff
{
	protected static StatusEffectManager effectManager;
	public float duration = 5;

	public abstract string EffectType { get; }

	private const bool hasTickEffect = false;

	public virtual bool HasTickEffect
	{
		get
		{
			return hasTickEffect;
		}
	}

	protected virtual bool RegisterEffect(StatusEffect copy, GameObject target)
	{
		if(!effectManager)
		{
			Debug.Log("Effect Manager assigned");
			effectManager = GameObject.Find("Game Manager").GetComponent<StatusEffectManager>();
		}
		return effectManager.RegisterEffect(copy, target);
	}

	public abstract void TickEffect(float delta);

	public abstract void RemoveEffect(GameObject target);

}