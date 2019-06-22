using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLoader : MonoBehaviour
{

	public GameObject weaponObj;
	public WeaponManager manager;

    // Start is called before the first frame update
    void Start()
    {
		manager = GameObject.Find("Game Manager").GetComponent<WeaponManager>();
    }

	public void Equip1()
	{
		manager.SetWeaponSlot(weaponObj, 1);
	}

	public void Equip2()
	{
		manager.SetWeaponSlot(weaponObj, 2);
	}

	public void Equip3()
	{
		manager.SetWeaponSlot(weaponObj, 3);
	}

}
