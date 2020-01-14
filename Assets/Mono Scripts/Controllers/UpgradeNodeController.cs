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
	[SerializeField] private int upgradeIdx;
	[SerializeField] private int baseCost = 10;
	[SerializeField] private int costIncrement = 10;

	private int currentLevel = 0 ;

	private void Start()
	{
		priceText.text = "" + Cost;
		iconButton.interactable = false;
	}

	private void Update()
	{
		if(Cost <= GameManager.Points && transmitter.CanUpgrade(upgradeIdx))
		{
			iconButton.interactable = true;
		}
		else
		{
			iconButton.interactable = false;
		}
	}

	//Using the values set in the inspector, a button press will attempt to
	//purchase the given upgrade, and sent it to the UpgradeTransmitter, which
	//causes the indicated UpgradeApplicator to increment the indicated upgrade (upgradeNum)
	public void PurchaseUpgrade()
	{
		if(transmitter.CanUpgrade(upgradeIdx) && GameManager.main.SpendPoints(Cost))
		{
			currentLevel++;
			transmitter.IncrementUpgrade(upgradeIdx);
			priceText.text = "" + Cost;

			//if the upgrade puts us at max, disable the upgrade button, have it indicate max value
			if (!transmitter.CanUpgrade(upgradeIdx))
			{
				priceText.text = "MAX";
				iconButton.interactable = false;
				ColorBlock colors = iconButton.colors;
				colors.disabledColor = new Color(0, 1, 0, 0.5f);
				iconButton.colors = colors;
			}
		}
	}

	private int Cost
	{
		get
		{
			return baseCost + currentLevel * costIncrement;
		}
	}

	/*	UPGRADE LIST
	 *	
	 *	0-10: General-Use Levels
	 *	
	 *		0-2: LauncherController
	 *			0: Charge Speed
	 *			1: Recharge Speed
	 *			2: Energy Capacity
	 *		
	 *		3-5: ProjectileController (all projectiles get these)
	 *			3: Mass Increase
	 *			4: Launch speed increase
	 *			5: Life span
	 *		
	 *		6-9: ExplosiveProjecile:
	 *			6: Detonator damage
	 *			7: Detonator impulse
	 *			8: Detonator radius
	 *			9: Detonator status effect level (not yet implemented, but may be useful)
	 *		
	 *		10: StatusProjectile Effect level
	 *	
	 *	11+: Projectile-Specific levels
	 *	
	 *		Splitslug:
	 *			11: Explosive
	 *			12: Pellet/Bounce Count
	 *			13: Spread Tightness
	 *		
	 *		Flare:
	 *			10: DPS Level (StatusProjectile effect level)
	 *			11: Burn radius (BurnZone)
	 *			12: Radial damage (BurnZone)
	 *			
	 *		Sticky:
	 *			11: Auto detonate
	 *		
	 *		Grav:
	 *			11: Radius
	 *			12: Strength
	 *			
	 *		TimeBomb:
	 *			11:	Fuse Speed
	 *			12: Detonator power
	 *			13: Contact Detonation
	 *			
	 *		Pulsar:
	 *			11: Explosion count
	 *		
	 */

}
