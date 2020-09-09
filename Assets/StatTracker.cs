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

        if (!PlayerPrefs.HasKey("Cumulative Score"))
            PlayerPrefs.SetInt("Cumulative Score", 0);

        if (!PlayerPrefs.HasKey("Shots Fired"))
            PlayerPrefs.SetInt("Shots Fired", 0);

        if (!PlayerPrefs.HasKey("Points Spent"))
            PlayerPrefs.SetInt("Points Spent", 0);

        if (!PlayerPrefs.HasKey("Bombs Detonated"))
            PlayerPrefs.SetInt("Bombs Detonated", 0);

        GameManager.GameStarted += OnGameStart;

    }

    protected void OnGameStart(object o, EventArgs e)
	{
        shotsFired = 0;
        bombKills = 0;
	}

    protected void OnGameEnd(object o, EventArgs e)
	{
        int stat = PlayerPrefs.GetInt("Cumulative Score");
        stat += GameManager.score;
        PlayerPrefs.SetInt("Cumulative Score", stat);

        stat = PlayerPrefs.GetInt("Shots Fired");
        stat += shotsFired;
        PlayerPrefs.SetInt("Shots Fired", stat);

        //points spent calculated as the difference between score and remaining points
        stat = PlayerPrefs.GetInt("Points Spent");
        stat += GameManager.score - GameManager.Points;
        PlayerPrefs.SetInt("Points Spent", stat);

        stat = PlayerPrefs.GetInt("Bombs Detonated");
        stat += bombKills;
        PlayerPrefs.SetInt("Bombs Detonated", stat);

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
