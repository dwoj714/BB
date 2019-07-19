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
		manager.SetWeaponSlot(manager.prefabs[PrefabIndex], WeaponManager.LeftIdx);
	}

	public void EquipMid()
	{
		Debug.Log("Equipping prefab from slot " + PrefabIndex);
		manager.SetWeaponSlot(manager.prefabs[PrefabIndex], WeaponManager.MiddleIdx);
	}

	public void EquipRight()
	{
		Debug.Log("Equipping prefab from slot " + PrefabIndex);
		manager.SetWeaponSlot(manager.prefabs[PrefabIndex], WeaponManager.RightIdx);
	}

	private int PrefabIndex
	{
		get
		{
			return rotor.Slots[rotor.Slots.Length / 2];
		}
	}

}
