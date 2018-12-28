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
	public GameObject gameOverButtons;
	public Text scoreText;

	[HideInInspector]
	public int score = 0;

	bool gameIsActive = false;

	private void Start()
	{
		launcherBase = GameObject.Find("Base").GetComponent<BaseController>();
		launcher = GameObject.Find("Launcher").GetComponent<LauncherController>();
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

	public void StartGame()
	{
		Debug.Log("StartGame()");
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
		mouseManager.controlledObject = "Launcher";
	}

	public void EndGame()
	{
		Debug.Log("EndGame()");
		gameIsActive = false;
		launcher.enabled = true;
		gameOverButtons.SetActive(true); 
		launcherBase.enabled = false;
		spawner.enabled = false;
		mouseManager.controlledObject = null;
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
}
