using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLoader : MonoBehaviour
{
	RotorController rotor;
	WeaponManager manager;

    // Start is called before the first frame update
    void Start()
    {
		manager = GameObject.Find("Game Manager").GetComponent<WeaponManager>();
		rotor = GameObject.Find("Weapon Wheel Rotor").GetComponent<RotorController>();
    }

	public void EquipLeft()
	{
		Debug.Log("Equipping prefab from slot " + PrefabIndex);
		manager.SetWeaponSlot(PrefabIndex, WeaponManager.LeftIdx);

		PlayerPrefs.SetInt("Left Weapon", PrefabIndex);
		PlayerPrefs.Save();
	}

	public void EquipMid()
	{
		Debug.Log("Equipping prefab from slot " + PrefabIndex);
		manager.SetWeaponSlot(PrefabIndex, WeaponManager.MiddleIdx);

		PlayerPrefs.SetInt("Mid Weapon", PrefabIndex);
		PlayerPrefs.Save();
	}

	public void EquipRight()
	{
		Debug.Log("Equipping prefab from slot " + PrefabIndex);
		manager.SetWeaponSlot(PrefabIndex, WeaponManager.RightIdx);

		PlayerPrefs.SetInt("Right Weapon", PrefabIndex);
		PlayerPrefs.Save();
	}

	private int PrefabIndex
	{
		get
		{
			return rotor.MedianIdx;
		}
	}

}
