using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorController : MonoBehaviour
{
	public float spread = 12;
	public float swapTime = 1;
	public Transform[] rotors = new Transform[3];

	private readonly int[] slots = { 0, 1, 2 };

	private Quaternion quatAlloc;

	private float amountMoved = 12;

	private bool cycling, cyclingLeft = false;

	private void Start()
	{
		rotors[0].SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, spread));
		rotors[1].SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));
		rotors[2].SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, -spread));
	}

	// Update is called once per frame
	void Update()
    {
 
    }

	IEnumerator CycleStep(bool left)
	{
		amountMoved = spread - amountMoved;

		float rotaionStep;
		bool halfway = false;
		do
		{
			float accMod = 2f - (1.5f * (amountMoved / spread));

			//The amount to move each frame to complete the swap over [swaptime] seconds
			rotaionStep = spread / swapTime * Time.deltaTime * (left ? -1 : 1) * accMod;
			amountMoved += Mathf.Abs(rotaionStep);

			//Halfway through the rotation...
			if(!halfway && amountMoved / spread > 0.5f)
			{
				halfway = true;
				int idx = left ? 2 : 0;

				//Move the offscreen rotor to the opposite side
				rotors[slots[idx]].Rotate(Vector3.forward, 3f * spread * (left ? 1 : -1));

				//set the order accordingly
				int a = slots[0];
				int b = slots[1];
				int c = slots[2];
				if (left)
				{
					slots[0] = c;
					slots[1] = a;
					slots[2] = b;
				}
				else
				{
					slots[0] = b;
					slots[1] = c;
					slots[2] = a;
				}
			}

			//when we've completed the rotation...
			if(amountMoved > spread)
			{
				//if we overstep the total amount to rotate, subtract the extra rotation from the final rotation step
				rotaionStep -= (amountMoved - spread) * (left ? -1 : 1);
				amountMoved = spread;
			}

			foreach (Transform rotor in rotors)
			{
				rotor.Rotate(Vector3.forward * rotaionStep, Space.Self);
			}

			yield return null;

		} while (amountMoved < spread);

		cycling = false;

	}

	public bool CycleLeft()
	{
		if (!(cycling && cyclingLeft))
		{
			StopAllCoroutines();
			cycling = true;
			cyclingLeft = true;
			StartCoroutine(CycleStep(true));
			return true;
		}
		return false;
	}

	public bool CycleRight()
	{
		if (!(cycling && !cyclingLeft))
		{
			StopAllCoroutines();
			cycling = true;
			cyclingLeft = false;
			StartCoroutine(CycleStep(false));
			return true;
		}
		return false;
	}


	[ExecuteInEditMode]
	private void OnDrawGizmos()
	{
		float scale = 0.5f;
		Gizmos.color = Color.blue;

		foreach(Vector3 pos in Positions)
		{
			Gizmos.DrawLine(pos + Vector3.left * scale, pos + Vector3.right * scale);
			Gizmos.DrawLine(pos + Vector3.up * scale, pos + Vector3.down * scale);
		}

	}

	public Vector3[] Positions
	{
		get
		{
			Vector3[] positions = { rotors[0].up * 15, rotors[1].up * 15, rotors[2].up * 15 };
			return positions;
		}
	}
}
