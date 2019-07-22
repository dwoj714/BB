using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

	[SerializeField]private Sprite settingEnabled, settingDisabled;

	//an array for each settins toggle button. Toggles the image indicating the status of the setting
	public Image[] toggleImages;

	//the menu that opened the settings menu. This is to be re-enabled on settings menu exit.
	//this can probably be set up with buttons.
	[HideInInspector] public GameObject activatingMenu;
	[SerializeField] private GameObject settingsMenu;

	private void Start()
	{
		settingsMenu.SetActive(false);
		//eventually add stuff to load saved settings
	}

	public void ToggleScreenShake()
	{
		CameraController.screenShakeEnabled = !CameraController.screenShakeEnabled;
		if (CameraController.screenShakeEnabled)
		{
			toggleImages[0].sprite = settingEnabled;
		}
		else
		{
			toggleImages[0].sprite = settingDisabled;
		}
	}

	public void ToggleBombHealth()
	{
		bool newStatus = !BombController.showHealth;
		BombController.showHealth = newStatus;

		foreach(BombController bomb in GameObject.FindObjectsOfType<BombController>())
		{
			bomb.text.enabled = newStatus;
		}

		if (newStatus)
		{
			toggleImages[1].sprite = settingEnabled;
		}
		else
		{
			toggleImages[1].sprite = settingDisabled;
		}

	}

	public void OpenMenu(GameObject activatingMenu)
	{
		this.activatingMenu = activatingMenu;
		activatingMenu.SetActive(false);
		settingsMenu.SetActive(true);
	}

	public void ExitMenu()
	{
		activatingMenu.SetActive(true);
		settingsMenu.SetActive(false);
	}

}
