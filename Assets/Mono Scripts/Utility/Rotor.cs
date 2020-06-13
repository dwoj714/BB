using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotor : MonoBehaviour
{

	[HideInInspector] public int index;
	[HideInInspector] public bool mid = false;
	[HideInInspector] public RotorController rotorController;

	protected virtual void Awake()
	{
		transform.localPosition = Vector3.zero;
	}

	public virtual float Rotation
	{
		get
		{
			return transform.localRotation.eulerAngles.z;
		}
		set
		{
			transform.localRotation = Quaternion.Euler(0, 0, value);
		}
	}

	public virtual void OnEnterMiddle()
	{
		mid = true;
	}
	public virtual void OnExitMiddle()
	{
		mid = false;
	}
}
