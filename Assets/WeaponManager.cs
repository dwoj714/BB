using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	[Header("Weapon prefabs")]
	public List<GameObject> prefabs = new List<GameObject>();

	public Transform[] slots = new Transform[3];

	public RotorController rotorController;

	InputManager inputMan;
	CircleCollider2D baseCol;

	private IInputReciever[] weapons = new IInputReciever[3];
	
	void Start()
	{
		inputMan = GameObject.Find("Game Manager").GetComponent<InputManager>();
		baseCol = GameObject.Find("Base").GetComponent<CircleCollider2D>();

		//instantiate up to 3 game objects from the start of the prefab list, assigning input recievers from the instances
		for(int i = 0; i < 3; i++)
		{
			if (prefabs[i])
			{
				Debug.Log("Instance ID of prefab[" + i + "]: " + prefabs[i].GetInstanceID());
				//weapons[i] = Instantiate(prefabs[i], slots[i]).GetComponent<IInputReciever>();
				SetWeaponSlot(prefabs[i], i, true);
			}
		}

		if (weapons[1] != null)
		{
			inputMan.reciever = weapons[1];
		}
		else
		{
			inputMan.reciever = weapons[0];
		}
	}

	void OnGameStart()
	{
		foreach(EnergyUser weapon in weapons)
		{
			weapon.energy = weapon.maxEnergy;
		}
	}

	public void SetWeaponSlot(GameObject pf, int i, bool isPrefab = false)
	{
		GameObject obj = Instantiate(pf, slots[i]);

		//attempt to get an input reciever from the object
		IInputReciever recv = obj.GetComponent<IInputReciever>();
		if (recv != null)
		{
			obj.transform.localPosition = Vector3.up * 15;

			//If there's already an object in the slot, destroy it
			if (weapons[i] != null)
			{
				Destroy(((MonoBehaviour)recv).gameObject);
			}

			weapons[i] = recv;

			//if the reciever is a launcher controller, set its collider accordingly
			try
			{
				LauncherController launcher = (LauncherController)recv;
				launcher.collider = baseCol;
			}
			catch (InvalidCastException)
			{
				Debug.Log("Non-launcher caught in SetWeaponSlot");
			}

		}
		else
		{
			Debug.LogError("No Input reciever found on object " + obj);
		}
	}

	public void Swap(bool left)
	{
		bool canSwap = false;

		if (left)
		{
			canSwap = rotorController.CycleLeft();
		}
		else
		{
			canSwap = rotorController.CycleRight();
		}

		if(canSwap)
		{
			weapons[1].OnInputCancel();

			IInputReciever a = weapons[0];
			IInputReciever b = weapons[1];
			IInputReciever c = weapons[2];

			if (left)
			{
				weapons[0] = c;
				weapons[1] = a;
				weapons[2] = b;
			}
			else
			{
				weapons[0] = b;
				weapons[1] = c;
				weapons[2] = a;
			}
			inputMan.reciever = weapons[1];
		}
	}
}
