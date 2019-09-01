using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeApplicator : MonoBehaviour
{
	[Header("Launcher Upgrades")]
	public string[] launcherUpgradeNames = { "Charge speed", "Recharge rate", "Max energy" };
	public int[] launcherUpgradeLevels = new int[3];

	//Each level is determined separately by the individual recieving the upgrade
	[Header("Projectile Upgrades")]
	public string[] projectileUpgradeNames = { "Mass", "Max speed", "Min speed" };	//3 values every projectile has
	public int[] upgradeLevels = new int[3];
	public int[] levelLimits = new int[3];

    public void OnShotLaunched(Launchable shot)
	{
		shot.SetUpgrades(upgradeLevels);

		foreach (IUpgradeable item in shot.GetComponentsInChildren<IUpgradeable>())
		{
			item.SetUpgrades(upgradeLevels);
		}

	}

	public void IncrementIndex(int i)
	{
		upgradeLevels[i]++;
	}

	public int UpgradeNameIndex(string str)
	{
		for(int i = 0; i < projectileUpgradeNames.Length; i++)
		{
			if(str == projectileUpgradeNames[i])
			{
				return i;
			}
		}
		Debug.LogError(name + ": Upgrade name \"" + str + "\" not found in upgrade name list");
		return -1;
	}

}
