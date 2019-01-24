using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler : MonoBehaviour {

	public static GameManager manager;
	public int pointValue = 10;

	// Use this for initialization
	void Start ()
	{
		//Set the game manager if it wasn't done in the inspector
		if (!manager)
		{
			manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
			Debug.Log("ScoreHandler Game Manager assigned");
		}

	}
	
	void OnExplosion()
	{
		manager.AddScore(pointValue);
	}

}
