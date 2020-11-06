using System;
using UnityEngine;

public class LauncherController : EnergyUser, IInputReciever, IUpgradeable
{
//////////////////////////   BEHAVIOR   //////////////////////////
	[Header("Behavior")]
	//Time needed to fully charge a shot
	public float chargeTime = 1;

	//Energy used per shot
	[SerializeField] private float shotEnergy = 5;

	//Projectile to be instantiated when firing
	[SerializeField] private Launchable ammo;

	[SerializeField] private bool launchWithAnimator = false;

//////////////////////////   STATICS   //////////////////////////

	public static bool invertAim = false;
	public static float stiffness = 1.0f;
	public static int InvertFactor
	{
		get
		{
			return invertAim ? -1 : 1;
		}
	}
	public static float maxDragLength = 2.5f;
	public static float minDragLength = 0.15f;
	private static IGController indicator = null;

//////////////////////////    UPGRADES STUFF    //////////////////
	
	[Header("Upgrades Per Level")]
	[SerializeField] private float chargeBonus;
	[SerializeField] private float rechargeBonus;
	[SerializeField] private float capacityBonus;

	[Header("Upgrades (See UpgradeNodeController for effects)")]
	private int[] upgradeLevels = new int[20];

	public int[] UpgradeLevels
	{
		get
		{
			return upgradeLevels;
		}
		set
		{
			value.CopyTo(upgradeLevels, 0);

			rechargeRate *= 1 + rechargeBonus * upgradeLevels[1];
			maxEnergy *= 1 + capacityBonus * upgradeLevels[2];
		}
	}

	[SerializeField] private int[] upgradeLimits;
	public int[] UpgradeLimits
	{
		get
		{
			return upgradeLimits;
		}
		set
		{
			upgradeLimits = value;
		}
	}

/////////////////////////////   INTERNAL   //////////////////////////////

	private float charge = 0;
	private Vector2 clickPos, dragPos, oldDrag;
	//private AnimationDispatcher animator;
	private Color bgColor;
	private SpriteRenderer iconSprite;
	[HideInInspector]public CircleCollider2D col;

////////////////////////////    PROPERTIES    ////////////////////////////

	public Launchable Shot { get; private set; }

	public Vector2 Drag
	{
		get
		{
			return dragPos - clickPos;
		}
	}

	public Vector2 OldDrag
	{
		get
		{
			return oldDrag - clickPos;
		}
	}

	public float ChargePercentage
	{
		get
		{
			return charge / chargeTime;
		}
	}

	public float ShotPower
	{
		get { return (Pull / maxDragLength * ChargePercentage).magnitude; }
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
			if ((pull.y > 0 && !invertAim) || (pull.y < 0 && invertAim))
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

////////////////////////////    EVENTS     ////////////////////////////

	public delegate void StaticShotFiredEventHandler(object source, EventArgs args);
	public static event StaticShotFiredEventHandler ShotFired;

	public delegate void ShotReadiedEventHandler(object source, ShotEventArgs args);
	public event ShotReadiedEventHandler ShotReadied;

	public delegate void ShotHeldEventHandler(object source, ShotEventArgs args);
	public event ShotHeldEventHandler ShotHeld;

	public delegate void ShotReleasedEventHandler(object source, ShotEventArgs args);
	public event ShotReleasedEventHandler ShotReleased;

	private ShotEventArgs args = new ShotEventArgs();

	////////////////////////////    METHODS    ////////////////////////////

	private void Start()
	{
		if (indicator == null)
		{
			indicator = GameObject.Find("Interaction Graphics").GetComponent<IGController>();
		}

		indicator.launcher = this;
		indicator.ChargeFieldVisible = false;
		InitEnergy();

		//animator = GetComponent<AnimationDispatcher>();

		iconSprite = GetComponentInChildren<SpriteRenderer>();
		bgColor = GetComponent<SpriteRenderer>().color;

	}

	private void LateUpdate()
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
		oldDrag = dragPos;
		dragPos = position;

		//move the scope if dragging past scope bounds based on stiffness
		if(Vector2.SqrMagnitude(clickPos - dragPos) > maxDragLength * maxDragLength)
		{
			Vector2 excess =  Drag - Vector2.ClampMagnitude(Drag, maxDragLength);
			Vector2 oldExcess = OldDrag - Vector2.ClampMagnitude(OldDrag, maxDragLength);

			if (oldExcess.sqrMagnitude < excess.sqrMagnitude)
				clickPos += (excess - oldExcess) * (1-stiffness);

			indicator.fSprite.transform.position = clickPos;
		}

		//If the input is dragged far enough, make the charge field visible and attempt to ready a shot. 
		if (!Armed && Drag.magnitude >= minDragLength)
		{
			if (!indicator.ChargeFieldVisible)
			{
				invertAim = Drag.y > 0;
				indicator.ChargeFieldVisible = true;
			}

			ReadyShot();
		}
		//If the launcher is armed, charge the shot until it's fully charged.
		else if (Armed)
		{
			IncrementCharge();
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
		if (!Armed && CanSpendEnergy(shotEnergy))//(!animator || animator.CancelAnimation()))
		{
			//create a copy of ammo, position it at the center of the launcher
			Shot = Instantiate(Ammo, transform.position + Vector3.up * transform.lossyScale.y / 2, Quaternion.identity).GetComponent<Launchable>();

			Shot.launcherCollider = col;

			Debug.Log("Base: " + Shot.launcherCollider + "\tShot: " + Shot.col);

			recharging = false;
			delayTimer = chargeTime;
			IncrementCharge();

			//Transfer upgrades to the shot, and any IUpgradeable components in its children
			Shot.UpgradeLevels = upgradeLevels;
			foreach (IUpgradeable item in Shot.GetComponentsInChildren<IUpgradeable>())
			{
				item.UpgradeLevels = upgradeLevels;
			}

			if (launchWithAnimator)
			{
				args.shot = Shot;
				args.direction = -Pull.normalized * InvertFactor;
				args.power = 0;

				ShotReadied?.Invoke(this, args);

			}

		}
	}

	private void AimShot()
	{
		if (launchWithAnimator)//animator != null && animator.enabled)
		{
			//Shot.transform.position = animator.ShotPosition;

			args.direction = -Pull.normalized * InvertFactor;
			args.power = ShotPower;
			ShotHeld?.Invoke(this, args);

		}
		else
		{
			Vector2 shotOffset = Pull.normalized * InvertFactor * (PullPercentage - 0.5f) * 2 * (transform.lossyScale.x / 2 - Shot.transform.lossyScale.x / 2);
			Shot.rb.MovePosition((Vector2)transform.position + shotOffset);
		}
	}

	private void LaunchShot()
	{
		//Notify event subscribers that a shot was fired
		ShotFired?.Invoke(this, EventArgs.Empty);

		if (Pull != Vector2.zero)
		{
			recharging = true;

			//If not launching with the animator, lau8nch the shot and play the animation
			//otherwise, hand the shot over to the animator for launch
			if (!launchWithAnimator)
			{
				Shot.Launch(-Pull.normalized * InvertFactor, ShotPower);

				/*if (animator != null && animator.enabled)
				{
					animator.OnShotLaunched();
				}*/
			}
			else
			{
				//animator.OnShotLaunched(Shot, -Pull.normalized * InvertFactor, ShotPower);

				args.direction = -Pull.normalized * InvertFactor;
				args.power = ShotPower;
				ShotReleased?.Invoke(this, args);

			}
			SpendEnergy(shotEnergy);

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
			Shot = null;
			charge = 0;

			//immediately resume recharging energy
			recharging = true;
			delayTimer = 0;
		}
	}

	public void OnGameStart()
	{
		for (int i = 0; i < upgradeLevels.Length; i++)
		{
			upgradeLevels[i] = 0;
		}
		energy = maxEnergy;
	}

	private void IncrementCharge()
	{
		if (charge < chargeTime)
		{
			charge += Time.deltaTime * (1 + chargeBonus * upgradeLevels[0]);

			if (charge > chargeTime)
			{
				charge = chargeTime;
			}
		}
	}

}

public class ShotEventArgs : EventArgs
{
	public ShotEventArgs()
	{
		shot = null;
		direction = Vector2.zero;
		power = 0;
	}

	public ShotEventArgs(Launchable shot, Vector2 direction, float power)
	{
		this.shot = shot;
		this.direction = direction;
		this.power = power;
	}

	public Launchable shot;
	public Vector2 direction;
	public float power;
}