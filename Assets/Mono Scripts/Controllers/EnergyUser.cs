using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnergyUser : MonoBehaviour
{

	[Header("Energy Pool")]
	public float maxEnergy = 100;
	public float rechargeRate = 5;
	protected bool recharging = true;
	[HideInInspector]
	public float energy;        //Holds energy amount
	protected float delayTimer;   //Counts down until energy can recharge after being used;

    // Update is called once per frame
    protected virtual void Update()
    {
		ProcessEnergy(); 
    }

	public void InitEnergy()
	{
		energy = maxEnergy;
	}

	//Handle energy calculations per update
	private void ProcessEnergy()
	{
		if (energy <= 0)
		{
			OnEnergyDeplete();
		}

		//If the recharge time is expired, increase energy if it's below max
		if (delayTimer <= 0 && recharging)
		{
			if (energy < maxEnergy)
			{
				energy += rechargeRate * Time.deltaTime;
			}
			else if (energy > maxEnergy)
			{
				energy = maxEnergy;
			}
		}
		else if (delayTimer > 0)
		{
			delayTimer -= Time.deltaTime;
		}
	}

	//This should do more stuff later on
	private void OnEnergyDeplete()
	{
		energy = 0;
	}

	//Returns false if the given energy cannot be spent
	public bool SpendEnergy(float cost)
	{
		if (cost <= energy)
		{
			energy -= cost;
			delayTimer = 0;
			return true;
		}
		else return false;
	}



	public float EnergyPercentage
	{
		get
		{
			return energy / maxEnergy;
		}
	}
}
