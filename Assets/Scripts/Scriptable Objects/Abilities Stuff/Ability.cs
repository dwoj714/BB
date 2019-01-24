using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
	[SerializeField]
	protected float cooldown;
	private float cdTimer = 0;
	[SerializeField]
	private float speedMultiplier = 1;

	public void UpdateCD()
	{
		if(cdTimer > 0)
		{
			cdTimer -= Time.deltaTime * speedMultiplier;
		}
	}

	public float Cooldown
	{
		get
		{
			return cooldown;
		}

		set
		{
			cooldown = value;
		}
	}

	public bool Ready
	{
		get
		{
			return cdTimer > 0;
		}
	}

	public float TimeUntilReady
	{
		get
		{
			return cdTimer / speedMultiplier;
		}
	}

	public float SpeedMultiplier
	{
		get
		{
			return speedMultiplier;
		}

		set
		{
			speedMultiplier = value;
		}
	}

	public void ClearCD()
	{
		cdTimer = 0;
	}

	public void StartCD()
	{
		cdTimer = cooldown;
	}
}
