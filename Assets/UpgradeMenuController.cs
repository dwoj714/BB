using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenuController : MonoBehaviour
{

	[SerializeField] private UpgradeTransmitter[] menuPrefabs;
	private UpgradeTransmitter[] spawnedMenus = new UpgradeTransmitter[3];

	private IUpgradeable launcher1, launcher2, launcher3;

	public void SpawnMenus()
	{
		//destroy existing menus
		for (int i = 0; i < 3; i++)
		{
			if (spawnedMenus[i]) Destroy(spawnedMenus[i].gameObject);
		}

		UpgradeTransmitter pf;

		//spawn new menus and assign upgrade applicators to the menus, typecasted from WeaponManager Input Recievers
		pf = menuPrefabs[WeaponManager.EquippedPrefabs[WeaponManager.LeftIdx]];
		spawnedMenus[WeaponManager.LeftIdx] = Instantiate(pf.gameObject, transform).GetComponent<UpgradeTransmitter>();
		spawnedMenus[WeaponManager.LeftIdx].target = ((MonoBehaviour)WeaponManager.Weapons[WeaponManager.LeftIdx]).GetComponent<LauncherController>();

		pf = menuPrefabs[WeaponManager.EquippedPrefabs[WeaponManager.MiddleIdx]];
		spawnedMenus[WeaponManager.MiddleIdx] = Instantiate(pf.gameObject, transform).GetComponent<UpgradeTransmitter>();
		spawnedMenus[WeaponManager.MiddleIdx].target = ((MonoBehaviour)WeaponManager.Weapons[WeaponManager.MiddleIdx]).GetComponent<LauncherController>();

		pf = menuPrefabs[WeaponManager.EquippedPrefabs[WeaponManager.RightIdx]];
		spawnedMenus[WeaponManager.RightIdx] = Instantiate(pf.gameObject, transform).GetComponent<UpgradeTransmitter>();
		spawnedMenus[WeaponManager.RightIdx].target = ((MonoBehaviour)WeaponManager.Weapons[WeaponManager.RightIdx]).GetComponent<LauncherController>();
	}

	//Activate the upgrade menu for the middle launcher
	private void OnEnable()
	{
		InputManager.main.inputHalt = true;
		InputManager.main.CancelInput();
		if (spawnedMenus[0])
		{
			spawnedMenus[WeaponManager.LeftIdx].gameObject.SetActive(false);
			spawnedMenus[WeaponManager.RightIdx].gameObject.SetActive(false);

			spawnedMenus[WeaponManager.MiddleIdx].gameObject.SetActive(true);
		}
	}

	//Need to make weapon levels reset on game start

	private void OnDisable()
	{
		//this always fails on frame 1 if enabled at game start, so don't bother
		try { InputManager.main.inputHalt = false; } catch (System.Exception) { return;}

		if (spawnedMenus[0])
		{
			spawnedMenus[WeaponManager.LeftIdx].gameObject.SetActive(false);
			spawnedMenus[WeaponManager.RightIdx].gameObject.SetActive(false);

			spawnedMenus[WeaponManager.MiddleIdx].gameObject.SetActive(false);
		}
	}

	public void ToggleActive()
	{
		gameObject.SetActive(!gameObject.activeInHierarchy);
	}

}
