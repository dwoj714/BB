using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : EnergyUser, IInputReciever
{

	public float shotCost;

	[Header("Aiming")]

	private float charge = 0;
	public float chargeTime = 1;
	
	private Vector2 clickPos, dragPos;
	public static float maxDragLength = 2.5f;
	public static float minDragLength = 0.15f;

	public IGController indicator;

	new public CircleCollider2D collider;

	//Projectile to be copied
	[SerializeField]
	private Launchable ammo;

	//Holds a reference to a copy of ammo, to be manipulated;
	private Launchable shot;

	public Vector2 Drag
	{
		get
		{
			return dragPos - clickPos;
		}
	}

	void Awake()
	{
		indicator.launcher = this;
		indicator.ChargeFieldVisible = false;
		InitEnergy();
	}

	void FixedUpdate()
	{
		if (Armed) AimShot();
	}

	public void OnInputStart(Vector2 position)
	{  
		clickPos = position;
		indicator.fSprite.transform.position = position;
	}

	public void OnInputHeld(Vector2 position)
	{
		dragPos = position;

		//If the input is dragged far enough, make the charge field visible and attempt to ready a shot. 
		if (!Armed && Drag.magnitude >= minDragLength)
		{
			indicator.ChargeFieldVisible = true;
			ReadyShot();
		}
		//If the launcher is armed, charge the shot until it's fully charged.
		else if (Armed)
		{
			if (charge < chargeTime)
			{
				charge += Time.deltaTime;

				if(charge > chargeTime)
				{
					charge = chargeTime;
				}
			}
		}
	}

	public void OnInputReleased(Vector2 position)
	{
		if (Armed)
		{
			LaunchShot();
		}
		indicator.ChargeFieldVisible = false;
	}

	public void OnInputCancel()
	{
		CancelShot();
		recharging = true;
		OnInputReleased(Vector2.zero);
		indicator.ChargeFieldVisible = false;
	}

	private void ReadyShot()
	{
		//If the required energy can be spent...
		if (!Armed && SpendEnergy(shotCost))
		{
			//create a copy of ammo, position it at the center of the launcher
			shot = Instantiate(Ammo, transform.position, Quaternion.identity).GetComponent<Launchable>();
			shot.launcherCollider = collider;
			recharging = false;
			delayTimer = chargeTime;
		}
	}

	private void AimShot()
	{
		shot.rb.MovePosition((Vector2)transform.position + Pull/ maxDragLength);
	}

	private void LaunchShot()
	{
		if (shot && Drag.sqrMagnitude >= Mathf.Pow(minDragLength, 2))
		{
			//launch the shot, re-enable energy recharge
			shot.Launch(-Pull, PullPercentage);
			recharging = true;
		}
		else
		{
			CancelShot();
		}

		//clear the shot variable, reset charge variable
		shot = null;
		charge = 0;
	}

	public void CancelShot()
	{
		if (shot)
		{
			//Destroy the shot object, refund the energy, and reset charge.
			Destroy(shot.gameObject);
			energy += shotCost;
			shot = null;
			charge = 0;

			//immediately resume recharging energy
			recharging = true;
			delayTimer = 0;
		}
	}

	public float ChargePercentage
	{
		get
		{
			return charge / chargeTime;
		}
	}

	public float ShotPower()
	{
		return (Drag / maxDragLength * ChargePercentage).magnitude;
	}

	public bool Armed
	{
		get
		{
			return shot;
		}
	}

	//Returns the drag value set by the input manager clamped to what the launcher allows for (bound by max length and current charge)
	public Vector2 Pull
	{
		get
		{
			return Vector2.ClampMagnitude(Drag, maxDragLength * ChargePercentage);
		}
	}

	public float PullPercentage
	{
		get
		{
			return Pull.magnitude / maxDragLength;
		}
	}

	public Launchable Ammo
	{
		get
		{
			return ammo;
		}

		set
		{
			ammo = value;
		}
	}

}
