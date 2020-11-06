using System.Collections;
using System;
using UnityEngine;

public class StatTracker : MonoBehaviour
{

    public static StatTracker main;

    private int shotsFired = 0;
    private int bombKills = 0;

    void Start()
    {
        if (!main) main = this;

        GameManager.GameStarted += OnGameStart;
        GameManager.GameEnded += OnGameEnd;

        BombController.BombDetonated += OnBombDetonated;
        LauncherController.ShotFired += OnShotFired;

    }

    protected void OnGameStart(object o, EventArgs e)
	{
        shotsFired = 0;
        bombKills = 0;
	}

    protected void OnGameEnd(object o, EventArgs e)
	{
        Debug.Log("Saving Stats...");

        int stat = PlayerPrefs.GetInt("Cumulative Score", 0);
        stat += GameManager.score;
        PlayerPrefs.SetInt("Cumulative Score", stat);

        stat = PlayerPrefs.GetInt("Shots Fired", 0);
        stat += shotsFired;
        PlayerPrefs.SetInt("Shots Fired", stat);

        //points spent calculated as the difference between score and remaining points
        stat = PlayerPrefs.GetInt("Points Spent", 0);
        stat += GameManager.score - GameManager.Points;
        PlayerPrefs.SetInt("Points Spent", stat);

        stat = PlayerPrefs.GetInt("Bombs Detonated", 0);
        stat += bombKills;
        PlayerPrefs.SetInt("Bombs Detonated", stat);

        stat = PlayerPrefs.GetInt("Games Played", 0);
        stat++;
        PlayerPrefs.SetInt("Games Played", stat);

        PlayerPrefs.Save();
	}

    protected void OnShotFired(object o, EventArgs e)
	{
        shotsFired++;
	}

    protected void OnBombDetonated(object o, EventArgs e)
	{
        bombKills++;
	}
}
