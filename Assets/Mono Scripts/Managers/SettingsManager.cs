using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	[SerializeField]private Sprite settingEnabled, settingDisabled = null;

	//an array for each settins toggle button. Toggles the image indicating the status of the setting
	public Image[] toggleImages;
	public Text[] optionText;
	public Slider[] sliders;

	//the menu that opened the settings menu. This is to be re-enabled on settings menu exit.
	//this can probably be set up with buttons.
	[HideInInspector] public GameObject activatingMenu;
	[SerializeField] private GameObject settingsMenu;

	private void Start()
	{
		settingsMenu.SetActive(false);
		SetSavedValues();
	}

	public void ToggleScreenShake()
	{
		ScreenShakeEnabled = !ScreenShakeEnabled;
	}

	public void ToggleBombHealth()
	{
		ShowBombHealth = !ShowBombHealth;
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
		SaveSettings();
	}

	private void SetSavedValues()
	{
		string read = PlayerPrefs.GetString("Toggle Settings");

		try
		{
			ScreenShakeEnabled = read[0] == '1';
			ShowBombHealth = read[1] == '1';
		} catch(System.IndexOutOfRangeException e)
		{
			Debug.Log(e + ": One or more settings not loaded");
		}

		Stiffness = PlayerPrefs.GetFloat("Stiffness", 0.8f);

	}

	public bool ScreenShakeEnabled
	{
		get
		{
			return CameraController.screenShakeEnabled;
		}
		set
		{
			CameraController.screenShakeEnabled = value;
			toggleImages[0].sprite = value ? settingEnabled : settingDisabled;
		}
	}

	public bool ShowBombHealth
	{
		get
		{
			return BombController.showHealth;
		}

		//Set the static showHealth component of BombController to true, and update existing bombs
		set
		{
			BombController.showHealth = value;
			foreach (BombController bomb in GameObject.FindObjectsOfType<BombController>())
			{
				bomb.UpdateHealthVisuals();
			}
			toggleImages[1].sprite = value ? settingEnabled : settingDisabled;
		}
	}

	public float Stiffness
	{
		get
		{
			return LauncherController.stiffness;
		}
		set
		{
			LauncherController.stiffness = value;
			sliders[3].value = value;
			PlayerPrefs.SetFloat("Stiffness", value);
		}
	}

	private void SaveSettings()
	{
		string str = "";
		str += ScreenShakeEnabled ? '1' : '0';
		str += ShowBombHealth ? '1' : '0';

		PlayerPrefs.SetString("Toggle Settings", str);
		PlayerPrefs.Save();
	}

}