using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	[Header("Weapon prefabs")]
	public List<GameObject> prefabs = new List<GameObject>();

	public RotorController weaponRotor;
	InputManager inputMan;
	CircleCollider2D baseCol;
	public IGController indicator;

	private static int activeWeapon = 1;
	public static WeaponManager main;

	private static int[] equippedPrefabs = new int[3];
	public static int[] EquippedPrefabs
	{
		get
		{
			return equippedPrefabs;
		}
		private set
		{
			equippedPrefabs = value;
		}
	}

	private static IInputReciever[] weapons = new IInputReciever[3];
	public static IInputReciever[] Weapons
	{
		get
		{
			return weapons;
		}
	}

	private void Awake()
	{
		//Debug.Log("WM: Checking for unlock string");
		if (!PlayerPrefs.HasKey("unlocks"))
		{
			Debug.Log("WM: Creating unlock string");
			//init unlocks to first 3 weapons are unlocked, rest are locked
			PlayerPrefs.SetString("unlocks", "UUULLLL");
			PlayerPrefs.Save();
		}
	}

	void Start()
	{
		inputMan = GameObject.Find("Game Manager").GetComponent<InputManager>();
		baseCol = GameObject.Find("Base").GetComponent<CircleCollider2D>();

		LoadWeapons();

		if (weapons[1] != null)
		{
			inputMan.reciever = weapons[1];
		}
		else
		{
			inputMan.reciever = weapons[0];
		}
		if (!main) main = this;
		else Debug.LogError("More than one WeaponManager spawned: " + name);

		GameManager.GameStarted += OnGameStart;
	}

	protected void OnGameStart(object o, EventArgs e)
	{
		foreach (LauncherController weapon in weapons)
		{
			weapon.OnGameStart();
		}

		inputMan.reciever = weapons[MiddleIdx];
		SetupIndicator();
	}

	//returns true if the given weapon index isn't locked
	public bool SetWeaponSlot(int pfIdx,int i)
	{
		//return false if the given weapon index is locked
		if(PlayerPrefs.GetString("unlocks", "UUULLLL")[pfIdx] == 'L')
		{
			return false;
		}
		StartCoroutine(SetWeaponSlotCo(pfIdx, i));
		return true;
	}

	//Method converted to coroutine to execute in steps, hopefully eliminating hitching
	private IEnumerator SetWeaponSlotCo(int pfIdx, int i)
	{
		//instantiate as a child of rotor in slot i
		GameObject obj = Instantiate(prefabs[pfIdx], weaponRotor.rotors[i].transform);

		yield return null;

		//attempt to get an input reciever from the object
		IInputReciever recv = obj.GetComponent<IInputReciever>();
		if (recv != null)
		{
			//move so it's at the edge of its rotor.
			obj.transform.localPosition = Vector3.up * 15;

			yield return null;

			//If there's already an object in the slot, destroy it
			if (weapons[i] != null)
			{
				Destroy(((MonoBehaviour)weapons[i]).gameObject);
			}

			weapons[i] = recv;
			EquippedPrefabs[i] = pfIdx;

			string debugStr = "wut";
			if (i == MiddleIdx)
				debugStr = "Middle";
			else if (i == LeftIdx)
				debugStr = "Left";
			else if (i == RightIdx)
				debugStr = "Right";

			Debug.Log("Weapons[" + i + "] (" + debugStr + ") prefab idx: " + pfIdx);

			yield return null;

			//if the reciever is a launcher controller, set its collider accordingly
			try
			{
				LauncherController launcher = (LauncherController)recv;
				launcher.col = baseCol;
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

	//to swap weapons, we cancel any active input functions, tell the Rotor to swap, and await the new InputReciever
	public void Swap(bool left)
	{
		//don't do anything while the game is paused
		if (GameManager.frozen) return;

		inputMan.reciever.OnInputCancel();

		weaponRotor.IncrementRotation(left);

		switch (activeWeapon)
		{
			case 0:
				activeWeapon = left ? 2 : activeWeapon + 1;
				break;

			case 1:
				activeWeapon += left ? -1 : 1;
				break;

			case 2:
				activeWeapon = left ? activeWeapon - 1 : 0;
				break;
		}
		inputMan.reciever = weapons[activeWeapon];
		SetupIndicator();

	}

	public static int MiddleIdx
	{
		get
		{
			return activeWeapon;
		}
	}

	public static int LeftIdx
	{
		get
		{
			if(activeWeapon == 0)
			{
				return 2;
			}
			else
			{
				return activeWeapon - 1;
			}
		}
	}

	public static int RightIdx
	{
		get
		{
			if (activeWeapon == 2)
			{
				return 0;
			}
			else
			{
				return activeWeapon + 1;
			}
		}
	}

	private void SetupIndicator()
	{
		try
		{
			LauncherController launcher = (LauncherController)weapons[activeWeapon];
			indicator.launcher = launcher;
		}
		catch (InvalidCastException e)
		{
			Debug.Log("Caught attempt to set non-launcher as IGController's launcher: " + e);
		}
	}

	public void LoadWeapons()
	{
		if(!SetWeaponSlot(PlayerPrefs.GetInt("Left Weapon", 0), LeftIdx))
		{
			SetWeaponSlot(0, LeftIdx);
		}

		if(!SetWeaponSlot(PlayerPrefs.GetInt("Mid Weapon", 1), MiddleIdx))
		{
			SetWeaponSlot(1, MiddleIdx);
		}
		
		if(!SetWeaponSlot(PlayerPrefs.GetInt("Right Weapon", 2), RightIdx))
		{
			SetWeaponSlot(2, RightIdx);
		}
	}

	private IEnumerator AwaitNewWeapon(float waitDuration)
	{
		float timer = 0;
		bool done = false;
		while (timer <= waitDuration && !done)
		{
			if(weaponRotor.MedianIdx != activeWeapon)
			{
				activeWeapon = weaponRotor.MedianIdx;
				done = true;
			}
			timer += Time.deltaTime;
			yield return null;
		}


	}

}
