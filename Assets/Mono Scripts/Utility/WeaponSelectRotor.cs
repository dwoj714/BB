using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectRotor : Rotor
{

	[SerializeField] private float percentCap = 0.7f;
	[SerializeField] private float selectedScale = 1;
	[SerializeField] private float offScale = 1;

	private Transform target;

	protected override void Awake()
	{
		base.Awake();

		target = transform.GetChild(0);

		if (mid)
		{
			TargetScale = selectedScale;
		}
		else
		{
			TargetScale = offScale;
		}
	}

	public override float Rotation
	{
		get
		{
			return base.Rotation;
		}

		set
		{
			base.Rotation = value;

			if (mid)
			{
				float diff = selectedScale - offScale;

				float scaleFactor = Mathf.Clamp01((1 - rotorController.PercentOffCenter) / percentCap);

				TargetScale = diff * scaleFactor + offScale;
			}

		}
	}

	public override void OnEnterMiddle()
	{
		base.OnEnterMiddle();
	}

	public override void OnExitMiddle()
	{
		base.OnExitMiddle();
		TargetScale = offScale;
	}

	private float TargetScale
	{
		get
		{
			return target.localScale.x;
		}

		set
		{
			target.localScale = Vector3.up * value + Vector3.right * value;
		}
	}

}