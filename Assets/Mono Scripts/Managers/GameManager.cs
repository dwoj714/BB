using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class GameManager : MonoBehaviour
{

	BaseController launcherBase;
	SpawnerController spawner;
	CameraController camController;
	public InputManager InputMan
	{
		get;
		private set;
	}

	IRandomList lastSpawnPool;

	[SerializeField] private GameEndSequencer gameOverMenu;
	[SerializeField] private GameObject mainMenu, inGameMenu, pauseMenu, loadoutMenuWorld, swapButtons;
	[SerializeField] private UpgradeMenuController upgradeMenu;
	//[SerializeField] private RotorController weaponWheel;
	[SerializeField] private DescriptionDisplayController loadoutMenuScreen;

	[Header("PhysCircle hitFX prefab")]
	[SerializeField] private GameObject hitFX;

	//used to restore altered time when pausing/unpausing game.
	private float storedTimeScale;

	[HideInInspector]
	public static int score = 0;
	private static int points = 0;

	public static int Points
	{
		get
		{
			return points;
		}
	}

	public static bool gameInProgress, frozen = false;

	//so things don't need to get a reference to this manager
	public static GameManager main;

	private void Awake()
	{
		PhysCircle.hitFX = hitFX;

		Application.targetFrameRate = 60;

		launcherBase = GameObject.Find("Base").GetComponent<BaseController>();
		spawner = GameObject.Find("Spawner").GetComponent<SpawnerController>();
		camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
		InputMan = GetComponent<InputManager>();

		mainMenu.SetActive(true);
		inGameMenu.SetActive(false);
		launcherBase.enabled = false;
		spawner.enabled = false;

		gameOverMenu.gameObject.SetActive(false);
		gameOverMenu.HideAll();

		upgradeMenu.Init();
		upgradeMenu.gameObject.SetActive(false);
		swapButtons.SetActive(false);
		loadoutMenuWorld.SetActive(false);
		loadoutMenuScreen.gameObject.SetActive(false);

		pauseMenu.SetActive(false);
		

		main = this;
	}

	public void StartGame()
	{

		GameEventManager.EventTriggered(GameEvent.GameStart);

		gameInProgress = true;
		inGameMenu.SetActive(true);
		score = 0;
		points = 0;

		//reset the game over menu;
		gameOverMenu.StopAllCoroutines();
		gameOverMenu.HideAll();
		gameOverMenu.gameObject.SetActive(false);

		mainMenu.SetActive(false);
		swapButtons.SetActive(true);
		loadoutMenuWorld.SetActive(false);
		loadoutMenuScreen.gameObject.SetActive(false);
		launcherBase.enabled = true;
		spawner.enabled = true;
		PurgeGameplayObjects();
		launcherBase.Restart();

		upgradeMenu.SetupMenus();

		camController.SetDestination("Game");
	}
	
	public void TogglePauseMenu()
	{
		if (gameInProgress)
		{
			if (pauseMenu.activeSelf)
			{
				SetFrozen(false);
				pauseMenu.SetActive(false);
			}
			else
			{
				if (frozen)
				{
					upgradeMenu.gameObject.SetActive(false);
					pauseMenu.SetActive(true);
				}
				else
				{
					SetFrozen(true);
					pauseMenu.SetActive(true);
				}
			}
		}
	}

	public void ToggleUpgradeMenu()
	{
		if (upgradeMenu.gameObject.activeSelf)
		{
			SetFrozen(false);
			upgradeMenu.gameObject.SetActive(false);
		}
		else
		{
			if (frozen)
			{
				pauseMenu.SetActive(false);
				upgradeMenu.Enable();
			}
			else
			{
				SetFrozen(true);
				upgradeMenu.Enable();
			}
		}
	}

	private void SetFrozen(bool frozen)
	{
		GameManager.frozen = frozen;
		if (frozen)
		{
			storedTimeScale = Time.timeScale;
			Time.timeScale = 0;
			AudioListener.pause = true;
		}
		else
		{
			Time.timeScale = storedTimeScale;
			AudioListener.pause = false;
		}
	}

	public void EndGame()
	{
		Debug.Log("EndGame()");

		if (PlayerPrefs.GetInt("Score 1", 0) < score)
		{
			PlayerPrefs.SetInt("Score 3", PlayerPrefs.GetInt("Score 2", 0));
			PlayerPrefs.SetInt("Score 2", PlayerPrefs.GetInt("Score 1", 0));
			PlayerPrefs.SetInt("Score 1", score);
		}
		else if (PlayerPrefs.GetInt("Score 2", 0) < score)
		{
			PlayerPrefs.SetInt("Score 3", PlayerPrefs.GetInt("Score 2", 0));
			PlayerPrefs.SetInt("Score 2", score);
		}
		else if (PlayerPrefs.GetInt("Score 3", 0) < score)
		{
			PlayerPrefs.SetInt("Score 3", score);
		}

		InputMan.CancelInput();
	
		gameInProgress = false;

		gameOverMenu.gameObject.SetActive(true);
		gameOverMenu.StartCoroutine("PlayEndSequence");

		PlayerPrefs.SetInt("bank", PlayerPrefs.GetInt("bank", 0) + Mathf.RoundToInt(score / 10.0f));
		PlayerPrefs.Save();

		launcherBase.enabled = false;
		spawner.enabled = false;
		swapButtons.SetActive(false);
		inGameMenu.SetActive(false);

		BroadcastMessage("OnGameEnd");

	}

	public void QuitGame()
	{
		InputMan.CancelInput();
		gameInProgress = false;
		launcherBase.enabled = false;
		spawner.enabled = false;
		swapButtons.SetActive(false);
		inGameMenu.SetActive(false);
		launcherBase.Charges = 0;
		BroadcastMessage("OnGameEnd");

		GoToMenu();
	}

	public void OnEscapePressed()
	{
		TogglePauseMenu();
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!frozen)
		{
			TogglePauseMenu();
		}
	}

	public void GoToMenu()
	{
		Debug.Log("GoToMenu()");

		//reset the game over menu;
		gameOverMenu.StopAllCoroutines();
		gameOverMenu.HideAll();
		gameOverMenu.gameObject.SetActive(false);

		inGameMenu.SetActive(false);
		loadoutMenuWorld.SetActive(false);
		loadoutMenuScreen.gameObject.SetActive(false);
		mainMenu.SetActive(true);
		PurgeGameplayObjects();
		launcherBase.Restart();

		//remove the input reciever
		//inputManager.recievers.Clear();
		camController.SetDestination("Menu");
	}

	public void GoToLoadout()
	{
		mainMenu.SetActive(false);
		camController.SetDestination("Loadout");
		swapButtons.SetActive(false);
		loadoutMenuWorld.SetActive(true);
		loadoutMenuScreen.gameObject.SetActive(true);

		LoadoutMenuSequencer.main.PlayEntry();
	}

	public void PurgeGameplayObjects()
	{
		StartCoroutine(ClearAll());
	}

	private IEnumerator ClearAll()
	{
		foreach (GameObject projectile in GameObject.FindGameObjectsWithTag("Projectile"))
		{
			Destroy(projectile);
		}
		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			Destroy(enemy);
		}

		yield return null; //wait a frame before deleting FX to get ones spawned by projectile destruction

		foreach (GameObject FX in GameObject.FindGameObjectsWithTag("FX"))
		{
			Destroy(FX);
		}
	}
	
	public void AddScore(int i)
	{
		if (gameInProgress)
		{
			score += i;
			points += i;
		}
	}

	public bool SpendPoints(int i)
	{
		if (i > points) return false;
		points -= i;
		return true;
	}

	public void DeleteData(string category)
	{
		switch (category)
		{
			case "unlocks":
				PlayerPrefs.DeleteKey(category);
				break;
			case "scores":
				PlayerPrefs.DeleteKey("Score1");
				PlayerPrefs.DeleteKey("Score2");
				PlayerPrefs.DeleteKey("Score3");
				break;
			case "bank":
				PlayerPrefs.DeleteKey("bank");
				break;
			case "all":
				PlayerPrefs.DeleteAll();
				break;
		}
		PlayerPrefs.Save();
	}
}
