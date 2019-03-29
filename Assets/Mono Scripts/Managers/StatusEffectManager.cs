using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
	//I'd LOVE to include the timers in this but you can't edit the values of a struct for some reason
	public struct EffectTargetPair
	{

		public EffectTargetPair(StatusEffect e, GameObject t)
		{
			effect = e;
			target = t;
		}

		public bool Equals(EffectTargetPair other)
		{
			return other.target == this.target && other.effect.EffectType == this.effect.EffectType;
		}

		public StatusEffect effect;
		public GameObject target;
	};

	private List<EffectTargetPair> activeEffects = new List<EffectTargetPair>();
	private List<float> timers = new List<float>();

	//Returns true if the given effect needs to be added to the target object (If it's not there already)
	public bool RegisterEffect(StatusEffect effect, GameObject target)
	{
		EffectTargetPair newPair = new EffectTargetPair(effect, target);

		//Check if the same effect is already on the given object
		for(int i = 0; i < activeEffects.Count; i++)
		{	
			//If an effect of the same type is on this object already... (determined in the .Equals method)
			if (activeEffects[i].Equals(newPair))
			{
				Debug.Log("Equivalent effect " + activeEffects[i].effect + " found on " + activeEffects[i].target);
				//If the duration of the new effect is greater than the remaining time on the old effect, reset the timer with the new duration
				if (newPair.effect.duration > timers[i])
				{
					timers[i] = newPair.effect.duration;
				}

				return false;
			}

		}

		//if the effect is not on the object, update the lists of timers and effect target pairs
		activeEffects.Add(new EffectTargetPair(effect, target));
		timers.Add(effect.duration);

		return true;
	}

	//Countdown all the timers, remove the effects of any expired timers, and delete expired timers and effects
	private void Update()
	{
		for (int i = 0; i < timers.Count; i++)
		{
			timers[i] -= Time.deltaTime;

			if (activeEffects[i].target && activeEffects[i].effect.HasTickEffect)
			{
				activeEffects[i].effect.TickEffect(Time.deltaTime);
			}

			if (timers[i] < 0)
			{
				if (activeEffects[i].target)
				{
					activeEffects[i].effect.RemoveEffect(activeEffects[i].target);
				}

				timers.RemoveAt(i);
				activeEffects.RemoveAt(i);
				i--;
			}
		}
	}
}
