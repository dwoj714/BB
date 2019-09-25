using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
	[SerializeField] int[] increaseChances = { 1, 1, 1, 1};

	public float interval = 15;
	private float timer = 0;

	private void Update()
	{
		if (GameManager.gameInProgress)
		{
			timer += Time.deltaTime;
			if(timer >= interval)
			{
				timer = 0;
				IncreaseDifficulty();
			}
		}
	}

	void IncreaseDifficulty()
	{
		int max = 0;
		foreach (int q in increaseChances)
		{
			max += q;
		}

		float selection = Random.Range(0, max);
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

	void IncreaseDifficulty(int i)
	{
		switch (i)
		{
			case 0:
				BombController.massMod += 0.1f;
				//Debug.Log("Bomb mass increased 10%");
				break;

			case 1:
				BombController.healthMod += 0.1f;
				//Debug.Log("Bomb health increased 10%");
				break;

			default:
				Debug.Log("What");
				break;
		}
	}

	void OnGameStart()
	{
		timer = 0;
		BombController.healthMod = 1;
		BombController.massMod = 1;
	}

	float WaveProgress
	{
		get
		{
			return timer / interval;
		}
	}

}
