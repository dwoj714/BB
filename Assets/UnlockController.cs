using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockController : MonoBehaviour
{
	[SerializeField] private int[] prices = new int[7];
	[SerializeField] private GameObject[] locks = new GameObject[7];

	[SerializeField] private RotorController weaponRotor;
	[SerializeField] private TextMeshProUGUI text;

	[SerializeField] private string equipText;
	[SerializeField] private string unlockText;
	[SerializeField] private string cantAffordText; //Implement this

	private bool swapWait = false;

	private void Start()
	{
		for (int i = 3; i < locks.Length; i++)
		{
			if (IndexUnlocked(i))
			{
				Destroy(locks[i]);
			}
		}

		UpdateText();

	}

	private void Update()
	{
		if (swapWait && weaponRotor.CycleProgress() > 0.5f)
		{
			UpdateText();
			swapWait = false;
		}
	}

	//returns true if the unlock is affordable and the index isn't already unlocked
	public bool CanUnlock(int idx)
	{
		if (PlayerPrefs.GetInt("bank", 0) >= prices[idx] && !IndexUnlocked(idx))
		{
			return true;
		}
		else return false;
	}

	public void OnWeaponCycle()
	{
		swapWait = true;
	}

	public static bool IndexUnlocked(int i)
	{
		return PlayerPrefs.GetString("unlocks", "UUULLLL")[i] == 'U';
	}

	public void UnlockWeapon(int idx)
	{
		if (CanUnlock(idx))
		{
			Destroy(locks[idx]);
			string list = PlayerPrefs.GetString("unlocks", "UUULLLL");
			string newList = "";

			for (int i = 0; i < list.Length; i++)
			{
				if (i == idx)
				{
					newList += 'U';
				}
				else
				{
					newList += list[i];
				}
			}

			PlayerPrefs.SetString("unlocks", newList);
			PlayerPrefs.SetInt("bank", PlayerPrefs.GetInt("bank", 0) - prices[idx]);
			PlayerPrefs.Save();

			UpdateText();

		}
	}

	private void UpdateText()
	{
		if (IndexUnlocked(weaponRotor.MedianIdx))
		{
			text.text = equipText;
		}
		else if(CanUnlock(weaponRotor.MedianIdx))
		{
			text.text = unlockText.Replace("~", prices[weaponRotor.MedianIdx].ToString());
		}
		else
		{
			text.text = cantAffordText.Replace("~", prices[weaponRotor.MedianIdx].ToString());
		}
	}

}
