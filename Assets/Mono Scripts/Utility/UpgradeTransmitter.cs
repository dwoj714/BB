using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpgradeTransmitter : MonoBehaviour
{
	public LauncherController target;

	public bool IncrementUpgrade(int i)
	{
		if (CanUpgrade(i))
		{
			target.UpgradeLevels[i]++;
			return true;
		}
		else return false;
	}

	public bool CanUpgrade(int i)
	{
		try
		{
			if (target.UpgradeLevels[i] < target.UpgradeLimits[i])
			{
				return true;
			}
			return false;
		}
		catch(System.IndexOutOfRangeException)
		{
			Debug.LogWarning(name + ": Attempting to get out of bounds upgrade index " + i);
			return false;
		}
	}

	private void ResetUpgrades()
	{
		for(int i = 0; i < target.UpgradeLevels.Length; i++)
		{
			target.UpgradeLevels[i] = 0;
		}
	}

	private void OnDestroy()
	{
		ResetUpgrades();
	}
}
