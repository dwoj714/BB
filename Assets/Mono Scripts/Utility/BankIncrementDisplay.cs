using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankIncrementDisplay : MonoBehaviour
{
	[SerializeField] private ScoreReader bankDisplay, depositDisplay;
	private int bank, deposit;

	public float transitionTime = 1;

	public static BankIncrementDisplay main;

	private void Awake()
	{
		main = this;
	}
	
	public void SetScores()
	{
		bank = PlayerPrefs.GetInt("bank");
		bankDisplay.SetScore(bank);

		deposit = Mathf.RoundToInt(GameManager.score / 10.0f);
		depositDisplay.SetScore(deposit);
	}

	public IEnumerator IncrementBank()
	{
		float timer = 0;
		float percentage;

		while(timer <= transitionTime)
		{
			timer += Time.deltaTime;
			if (timer > transitionTime) timer = transitionTime;

			percentage = timer / transitionTime;

			depositDisplay.SetScore((int)((1 - percentage) * deposit));
			bankDisplay.SetScore(bank + (int)(percentage * deposit));

			yield return null;
		}
	}

}
