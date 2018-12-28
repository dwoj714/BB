using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
	[HideInInspector]
	public Vector2 mouseDrag;

	public float maxDragLength = 5;
	[Range(0,.99f)]
	public float minimumPower;

	private float charge;

	//Used to tell projectiles what the base collider is
	public CircleCollider2D baseCollider;

	//Projectile to be copied
	public AmmoType ammo;

	//Holds a reference to a copy of ammo, to be manipulated;
	private AmmoType shot;

	public float maxEnergy = 100;
	public float rechargeSpeed = 5;
	public float rechargeDelay = .33f;
	[HideInInspector]
	public float energy;		//Holds energy amount
	private float delayTimer;	//Counts down until energy can recharge after being used;

	private void Start()
	{
		baseCollider = GameObject.Find("Base").GetComponent<CircleCollider2D>();
		energy = maxEnergy;
	}

	void Update()
	{
		ProcessEnergy();

		//If the mouse is down, increase charge by deltaTime while it's less than chargeTime
		if (shot && charge < shot.chargeTime)
		{
			charge += Time.deltaTime;

			if (charge > shot.chargeTime)
			{
				charge = shot.chargeTime;
			}
		}
	}

	void FixedUpdate()
	{
		if (shot) AimShot();
	}

	public void ReadyShot()
	{
		if (energy - ammo.energyCost >= 0)
		{
			//Reduce energy and pause the recharge of energy briefly
			energy -= ammo.energyCost;
			delayTimer = 0;

			//create a copy of ammo, position it at the center of the launcher
			shot = Instantiate(ammo, transform.position, Quaternion.identity);
			shot.launcherCollider = baseCollider;
		}
	}

	public void AimShot()
	{
		shot.rb.MovePosition((Vector2)transform.position + (mouseDrag * ChargePercentage()) / (maxDragLength + shot.AdjustedRadius * maxDragLength));
	}

	public void LaunchShot()
	{
		if (ShotPower() >= minimumPower && shot)
		{
			shot.Launch(-mouseDrag, ShotPower());
		}
		else
		{
			CancelShot();
		}
		//ComboCounter cc = shot.gameObject.AddComponent<ComboCounter>();

		shot = null;
		charge = 0;
	}

	public void CancelShot()
	{
		if (shot)
		{
			Destroy(shot.gameObject);
			shot = null;
			charge = 0;
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

	public float ChargePercentage()
	{
		return charge / ammo.chargeTime;
	}

	public float ShotPower()
	{
		return (mouseDrag / maxDragLength * ChargePercentage()).magnitude;
	}

	public bool Armed()
	{
		return shot;
	}

	private void OnEnergyDeplete()
	{

	}
}
