using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

	private float lateDamage;

	protected float health;
	public float maxHealth;

	protected float shield;
	public float maxShield;

	public float rechargeRate;
	public float rechargeDelay;

	private float counter;

	SpriteRenderer spr;

	protected virtual void Start()
	{
		health = maxHealth;
		shield = maxShield;

		spr = GetComponent<SpriteRenderer>();
	}

	protected virtual void Update()
	{
		//Check if shields need to recharge
		if (shield < maxShield)
		{
			//Increase the delay timer if recharge hasn't started
			if (counter < rechargeDelay)
			{
				counter += Time.deltaTime;
			}
			else
			{	
				//Increase shield until it reaches its maximum
				if(shield < maxShield)
				{
					shield += rechargeRate * Time.deltaTime;
				}
				if(shield > maxShield)
				{
					shield = maxShield;
				}
			}
		}
	}

	public void TakeDamage(float dmg, GameObject source = null)
	{

		//Debug.Log(name + " Damage Taken: " + dmg);

		if (shield > 0)
		{
			counter = 0;
			shield -= dmg;
			if (shield < 0)
			{
				health += shield;
				shield = 0;
			}
		}
		else
		{
			if (health >= 1)
			{
				health -= dmg;
			}
			if (health < 1)
			{
				health = 0;
				SendMessage("OnHealthDeplete");
			}
		}

		if (spr)
		{
			spr.material.SetFloat("_health", health / maxHealth);
		}
	}

	//Instead of taking damage instantly, store the damage amount and take the damage in the next frame update.
	//This should make explosion chains look like chain reactions
	//nope. Added a fuse. Maybe i'll keep this


	public void Heal(float healing)
	{
		if (health > 0)
		{
			health += healing;
			if (health > maxHealth)
			{
				health = maxHealth;
			}
			if (spr)
			{
				spr.material.SetFloat("_health", health / maxHealth);
			}
		}
	}

	public float getHealth()
	{
		return health;
	}

	public float getShield()
	{
		return shield;
	}

	public float HealthPercent()
	{
		return health / maxHealth;
	}
}
