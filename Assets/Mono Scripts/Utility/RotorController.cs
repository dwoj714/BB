using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorController : MonoBehaviour
{
	public float spread = 12;
	public float swapTime = 1;
	public Transform[] rotors = new Transform[3];

	[SerializeField] private int[] slots;

	public int[] Slots
	{
		get
		{
			return slots;
		}
	}

	private float amountMoved;

	private bool cycling, cyclingLeft = false;

	public int MedianIdx
	{
		get
		{
			return slots[slots.Length / 2];
		}
	}

	private void Awake()
	{
		int mid = rotors.Length / 2;
		for(int i = 0; i < rotors.Length; i++)
		{
			rotors[i].SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, (mid - i ) * spread));
		}

		slots = new int[rotors.Length];
		for(int i = 0; i < slots.Length; i++)
		{
			slots[i] = i;
		}

		amountMoved = spread;

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
				int idx = left ? rotors.Length - 1 : 0;

				//Move the offscreen rotor to the opposite side
				rotors[slots[idx]].Rotate(Vector3.forward, rotors.Length * spread * (left ? 1 : -1));

				int[] temp = (int[]) slots.Clone();

				if (left)
				{
					for(int i = 1; i < slots.Length; i++)
					{
						slots[i] = temp[i - 1];
					}
					slots[0] = temp[slots.Length - 1];

				}
				else
				{
					for(int i=0;i<slots.Length - 1; i++)
					{
						slots[i] = temp[i + 1];
					}
					slots[slots.Length - 1] = temp[0];

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

	public bool CanSwap
	{
		get
		{
			return !cycling;
		}
	}

	public void CycleLeftButton()
	{
		if (!cycling)
		{
			CycleLeft();
		}
	}

	public void CycleRightButton()
	{
		if (!cycling)
		{
			CycleRight();
		}
	}

	public float CycleProgress()
	{
		return amountMoved / spread;
	}

}
