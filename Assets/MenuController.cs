using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuController : MonoBehaviour
{
	public float rotateSpeed = 0.2f;

	[SerializeField]
	private float rotation = 0;

	public float Rotation
	{
		get
		{
			return rotation;
		}

		set
		{
			rotation = value;
			while (rotation >= 360)
				rotation -= 360;

			while (rotation < 0)
				rotation += 360;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{

		//Transform's current euler angles, z axis
		float ez = transform.eulerAngles.z;

		while (rotation < 0)
			rotation += 360;

		while (rotation - ez > 180)
			ez += 360;

		while (ez - rotation > 180)
			rotation += 360;

		//Perform the rotation toward rotation
		transform.eulerAngles = Vector3.forward * Mathf.Lerp(ez, rotation, rotateSpeed);
	}

	public void AddRoation(float degrees)
	{
		Rotation += degrees;
	}
}
