using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : PhysCircle {

	[Header("Energy Pool")]
	public float maxEnergy = 100;
	public float rechargeSpeed = 5;
	public float rechargeDelay = .33f;
	[HideInInspector]
	public float energy;        //Holds energy amount
	private float delayTimer;   //Counts down until energy can recharge after being used;

	[Header("Explosion Charges")]
	public int totalCharges = 3;
	public int charges;
	public bool trigger = false;

	SpriteRenderer spr;
	Detonator detonator;

	// Use this for initialization
	void Start ()
	{
		charges = totalCharges;
		detonator = GetComponent<Detonator>();
		rb.isKinematic = true;
		spr = GetComponent<SpriteRenderer>();
		energy = maxEnergy;
	}

	void Update()
	{
		ProcessEnergy();

		if (trigger)
		{
			trigger = false;
			spr.material.SetFloat("_startTime", Time.time);
		}
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
				energy += rechargeSpeed * Time.deltaTime;
			}
			else if (energy > maxEnergy)
			{
				energy = maxEnergy;
			}
		}
		else if (delayTimer < rechargeSpeed)
		{
			delayTimer += Time.deltaTime;
		}
	}

	//This should do more stuff later on
	private void OnEnergyDeplete()
	{
		energy = 0;
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		Detonator enemyDetonator = collision.collider.GetComponent<Detonator>();
																					//condition added to prevent sparked bombs from using a charge
		if (charges > 0 && !detonator.sparked && collision.collider.tag == "Enemy" && !enemyDetonator.sparkedLastFrame)
		{
			detonator.sparked = true;
			spr.material.SetFloat("_startTime", Time.time);
			spr.material.SetFloat("_angle", Vector2.Angle(collision.collider.transform.position, Vector2.right) * Mathf.Deg2Rad);
		}
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

	private void OnExplosion()
	{
		charges--;
	}

	public void Restart()
	{
		charges = totalCharges;
	}
}
