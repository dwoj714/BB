using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravUpgradeHandler : MonoBehaviour, IUpgradeable
{
	private int[] upgradeLevels;

	[SerializeField] private float radiusBonus = 0.3f;
	[SerializeField] private float strengthBonus = 0.2f;

	public int[] UpgradeLevels
	{
		get
		{
			return upgradeLevels;
		}
		set
		{
			upgradeLevels = value;
			transform.localScale *= 1 + upgradeLevels[11] * radiusBonus;
			GetComponent<PointEffector2D>().forceMagnitude *= 1 + upgradeLevels[12] * strengthBonus;
			GetComponent<VFXController>().timeScale *= 1 + upgradeLevels[12] * strengthBonus / 2;
		}
	}

}
