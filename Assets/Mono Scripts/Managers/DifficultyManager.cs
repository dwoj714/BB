﻿using System.Collections;
using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
	[SerializeField] private int[] increaseChances = { 1, 1, 1, 1};

	public float interval = 15;
	public static float Timer
	{
		get
		{
			return timer;
		}
	}
	private static float timer = 0;

	public static DifficultyManager main;

	public bool advance = true;

	[SerializeField] private ProgressIndicatorController progressBar;

	private void Start()
	{
		main = this;
		GameManager.GameEnded += OnGameEnd;
	}

	public void Update()
	{
		if (GameManager.gameInProgress)
		{
			
			if(advance) timer += Time.deltaTime;

			if(timer >= interval)
			{
				timer = 0;
				IncreaseDifficulty();
			}
		}
	}

	private void IncreaseDifficulty()
	{
		int max = 0;
		foreach (int q in increaseChances)
		{
			max += q;
		}

		float selection = UnityEngine.Random.Range(0, max);
		int chanceTotal = 0;
		bool decided = false;

		for (int i = 0; i < increaseChances.Length; i++)
		{
			chanceTotal += increaseChances[i];
			if (!decided && chanceTotal > selection)
			{
				decided = true;
				IncreaseDifficulty(i);
				//increaseChances[i]--;
			}
			else
			{
				increaseChances[i]++;
			}
		}
	}

	private void IncreaseDifficulty(int i)
	{
		switch (i)
		{
			case 0:
				BombController.massMod += 0.1f;
				StartCoroutine(progressBar.FlashMass());
				//Debug.Log("Bomb mass increased 10%");
				break;

			case 1:
				BombController.healthMod += 0.1f;
				StartCoroutine(progressBar.FlashHealth());
				//Debug.Log("Bomb health increased 10%");
				break;

			default:
				Debug.Log("What");
				break;
		}
	}

	private void OnGameEnd(object o, EventArgs e)
	{
		timer = 0;
		BombController.healthMod = 1;
		BombController.massMod = 1;
		progressBar.ResetProgress();
	}

	public float WaveProgress
	{
		get
		{
			return timer / interval;
		}
	}

}
