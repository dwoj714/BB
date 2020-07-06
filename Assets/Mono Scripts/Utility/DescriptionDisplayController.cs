using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionDisplayController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private RotorController rotorController;
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI descriptionText;

	private ProjectileInfo[] infoList = {

		new ProjectileInfo( "Splitslug",
			"A shell that fires a spread of pellets at low charge, and a bouncing slug at high charge"),

		new ProjectileInfo( "Flare",
			"A burning flare that continuously deals damage to anything near it"),

		new ProjectileInfo( "Impulse Charge",
			"A volatile projectile that sticks to bombs before exploding"),

		new ProjectileInfo( "Grav",
			"A gravity well that attracts anything near it"),

		new ProjectileInfo( "Mass Driver",
			"A large and extremely heavy projectile that can plow through almost anything"),

		new ProjectileInfo( "Slider Charge",
			"A heavy projectile that emits a powerful explosion a set time after launching"),

		new ProjectileInfo( "Pulsar",
			"An immaterial projectile which emits several explosions as it travels")
	};

	private int activeIdx = 0;

	private void Start()
	{
		activeIdx = rotorController.MedianIdx;
		
	}

	private void Update()
	{
		if(activeIdx != rotorController.MedianIdx)
		{
			activeIdx = rotorController.MedianIdx;
			SetDisplay(activeIdx);
		}
	}

	public void SetDisplay(int index)
	{
		nameText.text = infoList[index].name;
		descriptionText.text = infoList[index].description;
	}

	private struct ProjectileInfo
	{
		public string name;
		public string description;

		public ProjectileInfo(string name, string description)
		{
			this.name = name;
			this.description = description;
		}
	}

}
