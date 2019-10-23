using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Launchable : PhysCircle {

	[HideInInspector]
	public CircleCollider2D launcherCollider;

	protected override void Awake()
	{
		base.Awake();
		rb.isKinematic = true;
	}

	public abstract void Launch(Vector2 direction, float power);
}
