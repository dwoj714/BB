using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotor : MonoBehaviour
{

	public void Awake()
	{
		transform.localPosition = Vector3.zero;
	}

	public float Rotation
	{
		get
		{
			return transform.localRotation.z;
		}
		set
		{
			transform.localRotation = Quaternion.Euler(0, 0, value);
		}
	}

	public virtual void OnEnterMiddle() { }
	public virtual void OnExitMiddle() { }
}
