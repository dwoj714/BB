using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

	LauncherController launcher;
	BaseController launcherBase;
	Spawner spawner;
	CameraController camController;
	MouseInputManager mouseManager;
	IRandomList lastSpawnPool;
	public GameObject gameOverButtons;
	public Text scoreText;

	[HideInInspector]
	public int score = 0;

	bool gameIsActive = false;

	private void Start()
	{
		launcherBase = GameObject.Find("Base").GetComponent<BaseController>();
		launcher = GameObject.Find("Main Launcher").GetComponent<LauncherController>();
		spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
		camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
		mouseManager = GetComponent<MouseInputManager>();
		scoreText.text = score.ToString();
		scoreText.enabled = false;

		launcherBase.enabled = false;
		launcher.enabled = false;
		spawner.enabled = false;
		gameOverButtons.SetActive(false);
	}

	private void Update()
	{
		if (launcherBase.charges <= 0 && gameIsActive)
		{
			EndGame();
		}
	}

	//For use with buttons loading drop pools.chains
	public void StartGame(ScriptableObject obj)
	{
		//Surprised I can do this
		Debug.Log("Uh oh");
		StartGame((IRandomList)obj);
		Debug.Log("nvm?");
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
			Debug.Log("Loading DropPool");
			DropPool pool = ScriptableObject.CreateInstance<DropPool>();
			pool.CopyValues((DropPool)randList);
			spawner.Pool = pool;
		}
		else if (randList.GetType() == typeof(DropChain))
		{
			Debug.Log("Loading DropChain");
			DropChain chain = ScriptableObject.CreateInstance<DropChain>();
			chain.CopyValues((DropChain)randList);
			spawner.Chain = chain;
		}
		else Debug.Log("Chain or pool not given");

		gameIsActive = true;
		scoreText.enabled = true;
		score = 0;
		scoreText.text = score.ToString();
		gameOverButtons.SetActive(false);

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
		gameIsActive = false;
		launcher.enabled = true;
		gameOverButtons.SetActive(true); 
		launcherBase.enabled = false;
		spawner.enabled = false;
		mouseManager.ControlledObject = null;
		launcher.CancelShot();
	}

	public void GoToMenu()
	{
		Debug.Log("GoToMenu()");
		gameOverButtons.SetActive(false);
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
