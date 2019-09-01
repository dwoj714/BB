using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : EnergyUser, IInputReciever, IUpgradeable
{
	//[SerializeField] private float shotCost;
	[SerializeField] private float shotEnergy = 5;
	[Header("Aiming")]
	public static float maxDragLength = 2.5f;
	public static float minDragLength = 0.15f;
	private Vector2 clickPos, dragPos;

	private int[] upgrades = new int[3];

	private float charge = 0;
	public float chargeTime = 1;
	
	private static IGController indicator = null;
	public bool useDefaultGuide = true;

	new public CircleCollider2D collider;

	//Projectile to be copied
	[SerializeField]
	private Launchable ammo;
	public Launchable Shot { get; private set; }

	public Vector2 Drag
	{
		get
		{
			return dragPos - clickPos;
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
			return Shot;
		}
	}

	//Drag clamped to maximum allowed charge
	public Vector2 Pull
	{
		get
		{
			Vector2 pull = Vector2.ClampMagnitude(Drag, maxDragLength * ChargePercentage);
			if (pull.y > 0)
			{
				return Vector2.zero;
			}
			return pull;
		}
	}

	//Ratio of pull to max possible pull
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

	void Awake()
	{
		if (indicator == null)
		{
			indicator = GameObject.Find("Interaction Graphics").GetComponent<IGController>();
		}

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
		if (!Armed && SpendEnergy(shotEnergy))
		{
			//create a copy of ammo, position it at the center of the launcher
			Shot = Instantiate(Ammo, transform.position, Quaternion.identity).GetComponent<Launchable>();
			Shot.launcherCollider = collider;
			recharging = false;
			delayTimer = chargeTime;
		}
	}

	private void AimShot()
	{
		Shot.rb.MovePosition((Vector2)transform.position + Pull/ maxDragLength);
	}

	private void LaunchShot()
	{
		if (Pull.sqrMagnitude > 0)
		{
			//launch the shot, re-enable energy recharge
			SendMessage("OnShotLaunched", Shot);
			Shot.Launch(-Pull, PullPercentage);
			recharging = true;
		}
		else
		{
			CancelShot();
		}

		//clear the shot variable, reset charge variable
		Shot = null;
		charge = 0;
	}

	public void CancelShot()
	{
		if (Shot)
		{
			//Destroy the shot object, refund the energy, and reset charge.
			Destroy(Shot.gameObject);
			energy += shotEnergy;
			Shot = null;
			charge = 0;

			//immediately resume recharging energy
			recharging = true;
			delayTimer = 0;
		}
	}

	public void SetUpgrades(int[] upgrades)
	{
		for(int i = 0; i < this.upgrades.Length; i++)
		{
			this.upgrades[i] = upgrades[i];
		}
	}

}
