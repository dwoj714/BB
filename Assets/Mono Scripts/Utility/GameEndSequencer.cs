using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndSequencer : MonoBehaviour
{

	public RectTransform text, score, score1, score2, score3, button1, button2, bank;
	private Vector2 textPos, scorePos, score1Pos, score2Pos, score3Pos, button1Pos, button2Pos, bankPos;
	private bool setupDone = false;

	//because using Start/Awake() wasn't working right if not enabled on startup
	void Setup()
    {
		textPos = text.anchoredPosition;
		scorePos = score.anchoredPosition;
		score1Pos = score1.anchoredPosition;
		score2Pos = score2.anchoredPosition;
		score3Pos = score3.anchoredPosition;
		button1Pos = button1.anchoredPosition;
		button2Pos = button2.anchoredPosition;
		bankPos = bank.anchoredPosition;

		//HideAll();

		setupDone = true;
	}

	public void HideAll()
	{
		if (!setupDone)
		{
			Setup();
		}

		text.anchoredPosition = Vector2.down * textPos.y;
		score.anchoredPosition = Vector2.down * scorePos.y;

		score1.anchoredPosition = score1Pos + Vector2.left * score1.rect.width;
		score2.anchoredPosition = score2Pos + Vector2.left * score2.rect.width;
		score3.anchoredPosition = score3Pos + Vector2.left * score3.rect.width;

		button1.anchoredPosition = Vector2.up * button1Pos.y + Vector2.right * button1Pos.x * -1;
		button2.anchoredPosition = Vector2.up * button2Pos.y + Vector2.right * button2Pos.x * -1;

		bank.anchoredPosition = 2 * bankPos;
	}

	IEnumerator PlayEndSequence()
	{
		BankIncrementDisplay.main.SetScores();

		StartCoroutine(MoveText());

		float time = 0;
		while(time < 0.15f)
		{
			time += Time.deltaTime;
			yield return null;
		}

		StartCoroutine(MoveScore());

		time = 0;
		while (time < 0.1f)
		{
			time += Time.deltaTime;
			yield return null;
		}

		StartCoroutine(MoveButtons());

		time = 0;
		while (time < 0.2f)
		{
			time += Time.deltaTime;
			yield return null;
		}

		StartCoroutine(MoveScore1());
		StartCoroutine(MoveBank());

		time = 0;
		while (time < 0.1f)
		{
			time += Time.deltaTime;
			yield return null;
		}

		StartCoroutine(MoveScore2());

		time = 0;
		while (time < 0.1f)
		{
			time += Time.deltaTime;
			yield return null;
		}

		StartCoroutine(MoveScore3());

		time = 0;
		while (time < 2)
		{
			time += Time.deltaTime;
			yield return null;
		}

		BankIncrementDisplay.main.StartCoroutine(BankIncrementDisplay.main.IncrementBank());

	}

	IEnumerator MoveText()
	{
		Vector2 lerp = Vector2.zero;
		do
		{
			if (lerp != Vector2.zero) text.anchoredPosition = lerp;
			lerp = Vector2.Lerp(text.anchoredPosition, textPos, 12 * Time.deltaTime);
			yield return null;
		} while (!Mathf.Approximately(lerp.sqrMagnitude, text.anchoredPosition.sqrMagnitude));
	}

	IEnumerator MoveScore()
	{
		Vector2 lerp = Vector2.zero;
		do
		{
			if(lerp != Vector2.zero)score.anchoredPosition = lerp;
			lerp = Vector2.Lerp(score.anchoredPosition, scorePos, 12 * Time.deltaTime);
			yield return null;
		} while (!Mathf.Approximately(lerp.sqrMagnitude, score.anchoredPosition.sqrMagnitude));
	}
	
	IEnumerator MoveButtons()
	{
		Vector2 lerp = Vector2.zero;
		do
		{
			if (lerp != Vector2.zero)
			{
				button1.anchoredPosition = lerp;
				button2.anchoredPosition = Vector2.up * lerp.y + Vector2.left * lerp.x;
			}
			lerp = Vector2.Lerp(button1.anchoredPosition, button1Pos, 12 * Time.deltaTime);

			yield return null;
		} while (!Mathf.Approximately(lerp.sqrMagnitude, button1.anchoredPosition.sqrMagnitude));
	}

	IEnumerator MoveScore1()
	{
		Vector2 lerp = Vector2.zero;
		do
		{
			if (lerp != Vector2.zero) score1.anchoredPosition = lerp;
			lerp = Vector2.Lerp(score1.anchoredPosition, score1Pos, 12 * Time.deltaTime);
			yield return null;
		} while (!Mathf.Approximately(lerp.sqrMagnitude, score1.anchoredPosition.sqrMagnitude));
	}
	IEnumerator MoveScore2()
	{
		Vector2 lerp = Vector2.zero;
		do
		{
			if (lerp != Vector2.zero) score2.anchoredPosition = lerp;
			lerp = Vector2.Lerp(score2.anchoredPosition, score2Pos, 12 * Time.deltaTime);
			yield return null;
		} while (!Mathf.Approximately(lerp.sqrMagnitude, score2.anchoredPosition.sqrMagnitude));
	}
	IEnumerator MoveScore3()
	{
		Vector2 lerp = Vector2.zero;
		do
		{
			if (lerp != Vector2.zero) score3.anchoredPosition = lerp;
			lerp = Vector2.Lerp(score3.anchoredPosition, score3Pos, 12 * Time.deltaTime);
			yield return null;
		} while (!Mathf.Approximately(lerp.sqrMagnitude, score3.anchoredPosition.sqrMagnitude));
	}

	IEnumerator MoveBank()
	{
		Vector2 lerp = Vector2.zero;
		do
		{
			if (lerp != Vector2.zero) bank.anchoredPosition = lerp;
			lerp = Vector2.Lerp(bank.anchoredPosition, bankPos, 12 * Time.deltaTime);
			yield return null;
		} while (!Mathf.Approximately(lerp.sqrMagnitude, bank.anchoredPosition.sqrMagnitude));
	}

}
