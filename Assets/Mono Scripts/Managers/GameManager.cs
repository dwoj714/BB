using System;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class GameManager : MonoBehaviour
{

	LauncherController launcher;
	BaseController launcherBase;
	Spawner spawner;
	CameraController camController;
	InputManager mouseManager;
	IRandomList lastSpawnPool;
	public GameObject gameOverMenu, mainMenu, inGameMenu;
	public Text scoreText;
	

	[HideInInspector]
	public int score = 0;

	public static bool gameInProgress = false;

	private void Start()
	{
		launcherBase = GameObject.Find("Base").GetComponent<BaseController>();
		launcher = GameObject.Find("Main Launcher").GetComponent<LauncherController>();
		spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
		camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
		mouseManager = GetComponent<InputManager>();
		scoreText.text = score.ToString();

		inGameMenu.SetActive(false);
		launcherBase.enabled = false;
		launcher.enabled = false;
		spawner.enabled = false;
		gameOverMenu.SetActive(false);
		mainMenu.SetActive(true);
	}

	private void Update()
	{
		if (launcherBase.charges <= 0 && gameInProgress)
		{
			EndGame();
		}
	}

	//For use with buttons loading drop pools.chains
	public void StartGame(ScriptableObject obj)
	{
		//Surprised I can do this
		try
		{
			StartGame((IRandomList)obj);
		}
		catch (InvalidCastException e)
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

		launcher.enabled = true;
		launcherBase.enabled = true;
		spawner.enabled = true;
		PurgeGameplayObjects();
		launcherBase.Restart();
		mouseManager.ControlledObject = "Launcher";

		camController.SetDestination("Game");
	}

	public void EndGame()
	{
		Debug.Log("EndGame()");
		gameInProgress = false;
		launcher.enabled = true;
		gameOverMenu.SetActive(true); 
		launcherBase.enabled = false;
		spawner.enabled = false;
		mouseManager.ControlledObject = null;
		launcher.CancelShot();
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
		mainMenu.SetActive(true);
		camController.SetDestination("Menu");
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
		score += points;
		scoreText.text = score.ToString();
	}

	public void OnBombDestroy()
	{

	}

}
