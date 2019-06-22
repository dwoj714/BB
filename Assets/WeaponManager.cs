using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public GameObject obj1, obj2, obj3;
	private GameObject instance1, instance2, instance3;
	public Vector3 leftPos, middlePos, rightPos;

	//Holds weapons and their positions
	private Dictionary<IInputReciever, Vector3> slots;

	public float swapTime = 0.5f;
	private bool swapping, swappingLeft = false;
	private float swapTimer = 0;

	InputManager inputMan;
	CircleCollider2D baseCol;

	public IInputReciever weapon1, weapon2, weapon3;
	
	void Start()
	{
		slots.Add(weapon1, leftPos);
		slots.Add(weapon2, middlePos);
		slots.Add(weapon3, rightPos);

		inputMan = GameObject.Find("Game Manager").GetComponent<InputManager>();
		baseCol = GameObject.Find("Base").GetComponent<CircleCollider2D>();
		SetRecievers();
	}

	void Update()
	{
		if (swapping)
		{
			
		}
	}

	void OnGameStart()
	{
		SetWeaponSlot(obj1, 1);
		SetWeaponSlot(obj2, 2);
		SetWeaponSlot(obj3, 3);

		inputMan.reciever = weapon2;
	}

	public void SetWeaponSlot(GameObject obj, int i)
	{
		Vector3 pos = (i == 1 ? leftPos : (i == 2 ? middlePos : rightPos));
		GameObject instance = Instantiate(obj, pos, Quaternion.identity);

		IInputReciever recv = instance.GetComponent<IInputReciever>();

		try
		{
			LauncherController maybeLauncher = (LauncherController)recv;
			maybeLauncher.collider = baseCol;
		}
		catch (InvalidCastException)
		{
			Debug.Log("Non-launcher caught by SetWeaponSlot");
		}

		if (recv != null)
		{
			switch (i)
			{
				case 1:
					if (instance1)
					{
						Destroy(instance1);
					}
					instance1 = instance;
					//instance1 = instance;
					weapon1 = recv;
					break;
				case 2:
					if (instance2)
					{
						Destroy(instance2);
					}
					instance2 = instance;
					instance2 = instance;
					weapon2 = recv;
					break;
				case 3:
					if (instance3)
					{
						Destroy(instance3);
					}
					instance3 = instance;
					instance3 = instance;
					weapon3 = recv;
					break;
				default:
					Debug.Log("SetWeaponSlot: Invalid slot index");
					break;
			}
		}
		else
		{
			Destroy(instance);
			Debug.Log("SetWeaponSlot: No input reciever found on given GameObject");
		}
	}

	public void SetRecievers()
	{
		if(instance1) weapon1 = instance1.GetComponent<IInputReciever>();
		if(instance2) weapon2 = instance2.GetComponent<IInputReciever>();
		if(instance3) weapon3 = instance3.GetComponent<IInputReciever>();
	}

	//swaps middle and left weapon slot of leftSlot is true, swaps middle and right weapon slot otherwise
	public void BeginSwap(bool leftSlot)
	{
		if (swapping) return;
		swapping = true;
		swappingLeft = leftSlot;



		//Want to keep references within recievers constant
		/*
		IInputReciever holder = weapon2;

		if (leftSlot)
		{
			weapon2 = weapon1;
			weapon1 = holder;
		}
		else
		{
			weapon2 = weapon3;
			weapon3 = holder;
		}

		inputMan.reciever = weapon2;
		*/
	}

	[ExecuteInEditMode]
	private void OnDrawGizmos()
	{
		float scale = 0.5f;

		Gizmos.color = Color.blue;

		Gizmos.DrawLine(leftPos + Vector3.left * scale, leftPos + Vector3.right * scale);
		Gizmos.DrawLine(leftPos + Vector3.up * scale, leftPos + Vector3.down * scale);

		Gizmos.DrawLine(middlePos + Vector3.left * scale, middlePos + Vector3.right * scale);
		Gizmos.DrawLine(middlePos + Vector3.up * scale, middlePos + Vector3.down * scale);

		Gizmos.DrawLine(rightPos + Vector3.left * scale, rightPos + Vector3.right* scale);
		Gizmos.DrawLine(rightPos + Vector3.up * scale, rightPos + Vector3.down * scale);
	}

}
