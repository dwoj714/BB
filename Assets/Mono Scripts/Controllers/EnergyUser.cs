using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnergyUser : MonoBehaviour
{

	[Header("Energy Pool")]
	public float maxEnergy = 100;
	public float rechargeRate = 5;
	public float rechargeDelay = .33f;
	[HideInInspector]
	public float energy;        //Holds energy amount
	private float delayTimer;   //Counts down until energy can recharge after being used;

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

		//If the recharge time is expired, increase energy  if it's below max
		if (delayTimer >= rechargeDelay)
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
		else if (delayTimer < rechargeRate)
		{
			delayTimer += Time.deltaTime;
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
		if (cost < energy)
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
