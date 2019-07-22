using System;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class GameManager : MonoBehaviour
{

	BaseController launcherBase;
	Spawner spawner;
	CameraController camController;
	InputManager inputManager;
	WeaponManager weaponManager;

	IRandomList lastSpawnPool;

	[SerializeField] private GameObject gameOverMenu, mainMenu, inGameMenu, pauseMenu, loadoutMenu, swapButtons;
	public Text scoreText;

	//used to restore altered time when pausing/unpausing game.
	private float storedTimeScale;

	[HideInInspector]
	public static int score = 0;

	public static bool gameInProgress, paused = false;

	private void Start()
	{
		launcherBase = GameObject.Find("Base").GetComponent<BaseController>();
		spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
		camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
		inputManager = GetComponent<InputManager>();
		weaponManager = GetComponent<WeaponManager>();
		scoreText.text = score.ToString();

		mainMenu.SetActive(true);
		inGameMenu.SetActive(false);
		launcherBase.enabled = false;
		spawner.enabled = false;
		gameOverMenu.SetActive(false);
		swapButtons.SetActive(false);
		loadoutMenu.SetActive(false);
		pauseMenu.SetActive(false);
	}

	private void Update()
	{
		if (launcherBase.charges <= 0 && gameInProgress)
		{
			EndGame();
		}
	}

	//For use with buttons loading drop pools/chains
	public void StartGame(ScriptableObject obj)
	{
		try
		{
			StartGame((IRandomList)obj);
		}
		catch (InvalidCastException)
		{
			Debug.Log("Invalid object sent to StartGame(ScriptableObject)");
		}
	}

	public void StartGame()
	{
		StartGame(lastSpawnPool);
	}

	public void StartGame(IRandomList randList)
	{
		BroadcastMessage("OnGameStart", SendMessageOptions.DontRequireReceiver);
		spawner.OnGameStart();
		lastSpawnPool = randList;

		if (randList.GetType() == typeof(DropPool))
		{
			//Debug.Log("Loading DropPool");
			DropPool pool = ScriptableObject.CreateInstance<DropPool>();
			pool.CopyValues((DropPool)randList);
			spawner.Pool = pool;
		}
		else if (randList.GetType() == typeof(DropChain))
		{
			//Debug.Log("Loading DropChain");
			DropChain chain = ScriptableObject.CreateInstance<DropChain>();
			chain.CopyValues((DropChain)randList);
			spawner.Chain = chain;
		}
		//else Debug.Log("Chain or pool not given");

		gameInProgress = true;
		inGameMenu.SetActive(true);
		score = 0;
		scoreText.text = score.ToString();
		gameOverMenu.SetActive(false);
		mainMenu.SetActive(false);
		swapButtons.SetActive(true);
		loadoutMenu.SetActive(false);
		launcherBase.enabled = true;
		spawner.enabled = true;
		PurgeGameplayObjects();
		launcherBase.Restart();

		camController.SetDestination("Game");
	}

	public void PauseGame()
	{
		storedTimeScale = Time.timeScale;
		Time.timeScale = 0;

		pauseMenu.SetActive(true);
		paused = true;
	}

	public void ResumeGame()
	{
		Time.timeScale = storedTimeScale;
		pauseMenu.SetActive(false);
		paused = false;
	}

	public void TogglePaused()
	{
		if (paused)
		{
			ResumeGame();
		}
		else
		{
			PauseGame();
		}
	}

	public void EndGame()
	{
		Debug.Log("EndGame()");
		gameInProgress = false;
		gameOverMenu.SetActive(true); 
		launcherBase.enabled = false;
		spawner.enabled = false;
		swapButtons.SetActive(false);
		launcherBase.charges = 0;
	}

	public void QuitGame()
	{
		EndGame();
		GoToMenu();
	}

	public void GoToMenu()
	{
		Debug.Log("GoToMenu()");
		gameOverMenu.SetActive(false);
		inGameMenu.SetActive(false);
		loadoutMenu.SetActive(false);
		mainMenu.SetActive(true);
		PurgeGameplayObjects();
		camController.SetDestination("Menu");
	}

	public void GoToLoadout()
	{
		mainMenu.SetActive(false);
		camController.SetDestination("Loadout");
		swapButtons.SetActive(false);
		loadoutMenu.SetActive(true);
	}

	public void PurgeGameplayObjects()
	{
		Debug.Log("PurgeGameplayObjects()");
		foreach (GameObject projectile in GameObject.FindGameObjectsWithTag("Projectile"))
		{
			Destroy(projectile);
		}
		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			Destroy(enemy);
		}
	}
	
	public void AddScore(int points)
	{
		if (gameInProgress)
		{
			score += points;
			scoreText.text = score.ToString();
		}
	}

	public void OnBombDestroy()
	{

	}

}
