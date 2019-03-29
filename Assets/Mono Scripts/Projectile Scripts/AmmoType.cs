using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AmmoType : PhysCircle {

	[HideInInspector]
	public CircleCollider2D launcherCollider;

	protected override void Awake()
	{
		base.Awake();
		rb.isKinematic = true;
	}

	public float chargeTime = 1;
	public float energyCost = 10;

	public abstract void Launch(Vector2 direction, float power);

}
