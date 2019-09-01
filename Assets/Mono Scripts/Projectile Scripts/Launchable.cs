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

	public abstract void Launch(Vector2 direction, float power);

	public abstract void SetUpgrades(int[] upgradeLevels);

	//public abstract void ApplyUpgrade(string name);
	//public abstract void RemoveUpgrade(string name);
	//public abstract bool UpgradeEnabled(string name);
	//public abstract void RemoveAllUpgrades();

}
