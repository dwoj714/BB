using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
	[HideInInspector]
	private Vector2 drag;

	public float maxDragLength = 5;
	public float minDragLength = 0.25f;

	private float charge;

	//Can't just call it 'base' :/
	public BaseController baseCon;

	//Projectile to be copied
	[SerializeField]
	private AmmoType ammo;

	//Holds a reference to a copy of ammo, to be manipulated;
	private AmmoType shot;



	public Vector2 Drag
	{
		get
		{
			return drag;
		}

		set
		{
			drag = value;
		}
	}

	private void Start()
	{

	}

	void Update()
	{
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
		//If the required energy can be spent...
		if (baseCon.SpendEnergy(Ammo.energyCost))
		{
			//create a copy of ammo, position it at the center of the launcher
			shot = Instantiate(Ammo, transform.position, Quaternion.identity);
			shot.launcherCollider = baseCon.col;
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
			baseCon.energy += shot.energyCost;
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
			return Vector2.ClampMagnitude(drag, maxDragLength * ChargePercentage);
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
