using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : EnergyUser, IInputReciever
{

	public IGController indicator;

	Vector2 clickPos, dragPos;

	public float maxDragLength = 5;
	public float minDragLength = 0.25f;

	private float charge;

	private float chargeTime;

	public CircleCollider2D collider;

	//Projectile to be copied
	[SerializeField]
	private AmmoType ammo;

	//Holds a reference to a copy of ammo, to be manipulated;
	private AmmoType shot;

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

	protected override void Update()
	{
		base.Update();

		if(Input.GetMouseButton(0) && ChargePercentage != 1)
		{
			chargeTime += Time.deltaTime;
		}
		if(Input.GetMouseButtonUp(0))
		{
			//Debug.Log("Time for charge: "+chargeTime);
			chargeTime = 0;
		}

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

	void OnGameStart()
	{
		energy = maxEnergy;
	}

	void FixedUpdate()
	{
		if (shot) AimShot();
	}

	public void OnInputStart(Vector2 position)
	{  
		clickPos = position;
		indicator.fSprite.transform.position = position;
	}

	public void OnInputHeld(Vector2 position)
	{
		dragPos = position;

		if (!Armed() && Drag.magnitude >= minDragLength)
		{
			indicator.ChargeFieldVisible = true;
			ReadyShot();
		}
	}

	public void OnInputReleased(Vector2 position)
	{
		if (Armed())
		{
			LaunchShot();
		}
		indicator.ChargeFieldVisible = false;
	}

	public void ReadyShot()
	{
		//If the required energy can be spent...
		if (!Armed() && SpendEnergy(Ammo.energyCost))
		{
			//create a copy of ammo, position it at the center of the launcher
			shot = Instantiate(Ammo, transform.position, Quaternion.identity).GetComponent<AmmoType>();
			shot.launcherCollider = collider;
		}
	}

	public void AimShot()
	{
		shot.rb.MovePosition((Vector2)transform.position + Pull/ maxDragLength);
	}

	public void LaunchShot()
	{
		if (shot && Drag.sqrMagnitude >= Mathf.Pow(minDragLength, 2))
		{
			shot.Launch(-Pull, PullPercentage);
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
			energy += shot.energyCost;
			shot = null;
			charge = 0;
		}
	}

	public float ChargePercentage
	{
		get
		{
			return charge / Ammo.chargeTime;
		}
	}

	public float ShotPower()
	{
		return (Drag / maxDragLength * ChargePercentage).magnitude;
	}

	public bool Armed()
	{
		return shot;
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

	public AmmoType Ammo
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
