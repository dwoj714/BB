using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNodeController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Button iconButton;
	[SerializeField] private Text priceText;
	[SerializeField] private UpgradeTransmitter transmitter;

	[Header("Upgrade Properties")]
	[SerializeField] private string upgradeName = "Mass";
	[SerializeField] private int upgradeLimit = 1;
	[SerializeField] private int baseCost = 10;
	[SerializeField] private int costIncrement = 10;

	private int currentLevel = 0 ;

	private void Update()
	{
		if(Cost >= GameManager.Points)
		{
			iconButton.interactable = true;
		}
		else
		{
			iconButton.interactable = false;
		}
	}

	public void PurchaseUpgrade()
	{
		if (GameManager.Points >= Cost)
		{
			GameManager.SpendPoints(Cost);
			currentLevel++;
			transmitter.IncrementApplicatorName(upgradeName);
		}
	}

	private int Cost
	{
		get
		{
			return baseCost + currentLevel * costIncrement;
		}
	}

}
