using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

	protected float health;
	public float maxHealth;

	protected float shield;
	public float maxShield;

	public float rechargeRate;
	public float rechargeDelay;

	private float counter;

	IHealable healthTarget;

	protected virtual void Start()
	{
		health = maxHealth;
		shield = maxShield;

		healthTarget = GetComponent<BombController>();
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
				shield = 0;
			}
		}
		else
		{
			if (health > 0)
			{
				health -= dmg;
				if(healthTarget != null) healthTarget.OnTakeDamage(dmg);
			}
			if (health <= 0)
			{
				health = 0;
				if (healthTarget != null) healthTarget.OnHealthDeplete();
			}
		}
	}

	public void Heal(float healing)
	{
		if (health > 0)
		{
			health += healing;
			if (health > maxHealth)
			{
				health = maxHealth;
			}
			if (healthTarget != null) healthTarget.OnHeal(healing);
		}
	}

	public float Health
	{
		get
		{
			return health;
		}
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
