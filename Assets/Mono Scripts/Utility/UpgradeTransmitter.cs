using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTransmitter : MonoBehaviour
{
	public UpgradeApplicator applicator;
	public LauncherController launcher;

	public void IncrementApplicatorIndex(int i)
	{
		applicator.IncrementIndex(i);
	}
	public void IncrementApplicatorName(string str)
	{
		IncrementApplicatorIndex(applicator.UpgradeNameIndex(str));
	}

	public void IncrementLauncherUpgrade(int i)
	{

	}
	
}
