using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : GameEventListener
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
    }


    public override void Notify(GameEvent eventType)
	{
		switch (eventType)
		{
            case GameEvent.GameStart:
                OnGameStart();
                break;
            case GameEvent.GameEnd:
                OnGameEnd();
                break;
            case GameEvent.ShotFired:
                OnShotFired();
                break;
            case GameEvent.BombDetonated:
                OnBombDetonated();
                break;
        }
	}

    private void OnGameStart()
	{
        shotsFired = 0;
        bombKills = 0;
	}

    private void OnGameEnd()
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

    private void OnShotFired()
	{
        shotsFired++;
	}

    private void OnBombDetonated()
	{
        bombKills++;
	}
}
