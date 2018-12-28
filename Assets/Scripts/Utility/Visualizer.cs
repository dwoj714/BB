using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Visualizer : MonoBehaviour {

	public bool mobile = false;
	PhysCircle parentCircle;

	protected float initialScaleX;

	//Ok so you needed this to avoid adding localPosition directly from the transform
	//because its values can change slightly during (most likely) physics
	//calculations, and apparently somehow allows floating point imprecision
	//to appear, add up exponentially, and "lock" the health bar directly on top of the parent transform
	protected Vector3 initialLocalPosition;

	// Use this for initialization
	protected virtual void Start ()
	{
		initialLocalPosition = transform.localPosition;
		initialScaleX = transform.localScale.x;
		
		if(mobile)parentCircle = transform.parent.GetComponent<PhysCircle>();
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		if (parentCircle)
		{
			transform.rotation = Quaternion.identity;
			transform.position = transform.parent.position + Vector3.up * initialLocalPosition.y;
		}
	}
}
