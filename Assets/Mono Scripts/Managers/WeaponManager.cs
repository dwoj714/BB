using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	[Header("Weapon prefabs")]
	public List<GameObject> prefabs = new List<GameObject>();

	[SerializeField] private Transform[] slots = new Transform[3];
	public static int[] equippedPrefabs = new int[3];
	public RotorController rotorController;
	InputManager inputMan;
	CircleCollider2D baseCol;
	public IGController indicator;
	private IInputReciever[] weapons = new IInputReciever[3];
	private static int activeWeapon = 1;

	//[SerializeField] private DotSight defaultSight;

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
	}

	void OnGameStart()
	{
		foreach (EnergyUser weapon in weapons)
		{
			weapon.energy = weapon.maxEnergy;
		}

		inputMan.reciever = weapons[MiddleIdx];
		SetupIndicator();


		//UpgradeTransmitter container;

		//for each weapon, assign the references of relevant UpgradeTransmitter objects so they
		//can interact with the attached UpgradeApplicators.
		//for(int i=0;i<equippedPrefabs.Length;i++)
		//{
		//	try
		//	{
				
		//		switch (equippedPrefabs[i])
		//		{
		//			//splitslug
		//			case 0:
		//				Debug.Log("re");
		//				GameObject f = GameObject.;

		//				Debug.Log(f);
		//				container = f.GetComponent<UpgradeTransmitter>();

		//				Debug.Log("reee");
		//				container.applicator = ((MonoBehaviour)weapons[i]).GetComponent<UpgradeApplicator>();
		//				break;
		//			//flare
		//			case 1:
		//				container = GameObject.Find("Flare Upgrades").GetComponent<UpgradeTransmitter>();
		//				container.applicator = ((MonoBehaviour)weapons[i]).GetComponent<UpgradeApplicator>();
		//				break;
		//			//sticky
		//			case 2:
		//				container = GameObject.Find("Impulse Charge Upgrades").GetComponent<UpgradeTransmitter>();
		//				container.applicator = ((MonoBehaviour)weapons[i]).GetComponent<UpgradeApplicator>();
		//				break;
		//			//grav
		//			case 3:
		//				container = GameObject.Find("Grav Upgrades").GetComponent<UpgradeTransmitter>();
		//				container.applicator = ((MonoBehaviour)weapons[i]).GetComponent<UpgradeApplicator>();
		//				break;
		//			//mass driver
		//			case 4:
		//				container = GameObject.Find("Mass Driver Upgrades").GetComponent<UpgradeTransmitter>();
		//				container.applicator = ((MonoBehaviour)weapons[i]).GetComponent<UpgradeApplicator>();
		//				break;
		//			//time bomb
		//			case 5:
		//				container = GameObject.Find("Time Bomb Upgrades").GetComponent<UpgradeTransmitter>();
		//				container.applicator = ((MonoBehaviour)weapons[i]).GetComponent<UpgradeApplicator>();
		//				break;
		//			//pulsar
		//			case 6:
		//				container = GameObject.Find("Pulsar Upgrades").GetComponent<UpgradeTransmitter>();
		//				container.applicator = ((MonoBehaviour)weapons[i]).GetComponent<UpgradeApplicator>();
		//				break;
		//		}
		//	} catch (NullReferenceException) { Debug.Log("Failed to locate upgrade transmitter"); }
		//
		//}

	}

	//returns true if the given weapon index isn't locked
	public bool SetWeaponSlot(int pfIdx,int i)
	{
		//return false if the given weapon index is locked
		if(PlayerPrefs.GetString("unlocks", "UUULLLL")[pfIdx] == 'L')
		{
			return false;
		}

		//instantiate as a child of rotor in slot i
		GameObject obj = Instantiate(prefabs[pfIdx], slots[i]);

		//attempt to get an input reciever from the object
		IInputReciever recv = obj.GetComponent<IInputReciever>();
		if (recv != null)
		{
			//move so it's at the edge of its rotor.
			obj.transform.localPosition = Vector3.up * 15;

			//If there's already an object in the slot, destroy it
			if (weapons[i] != null)
			{
				Destroy(((MonoBehaviour)weapons[i]).gameObject);
			}

			weapons[i] = recv;
			equippedPrefabs[i] = pfIdx;

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
		return true;
	}

	public void Swap(bool left)
	{
		inputMan.reciever.OnInputCancel();

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

			//if (launcher.useDefaultGuide)
			//{
			//	defaultSight.Launcher = launcher;
			//}

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

}
