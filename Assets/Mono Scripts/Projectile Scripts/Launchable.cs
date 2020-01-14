using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Launchable : PhysCircle, IUpgradeable
{
	[HideInInspector]
	public CircleCollider2D launcherCollider;

	protected override void Awake()
	{
		base.Awake();
		rb.isKinematic = true;
	}

	public abstract float LaunchSpeed(float power);

	public abstract void Launch(Vector2 direction, float power);

	protected int[] upgradeLevels = new int[6];
	public virtual int[] UpgradeLevels
	{
		get
		{
			return upgradeLevels;
		}
		set
		{
			upgradeLevels = value;
		}
	}
}
