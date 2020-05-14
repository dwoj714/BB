﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreReader : MonoBehaviour
{
	[TextArea(15,10)]
	public string formatText = "~";
	private int score = -1;

	[SerializeField] private string otherSource = "";

	[SerializeField] private bool readPoints = false;

	//constantly update this score text in Update()
	[SerializeField] private bool updateInUpdate;

	public ScoreCategory readSource = ScoreCategory.Current;

	private TextComponent text;

	private void Awake()
	{
		text = new TextComponent(GetComponent<Text>(), GetComponent<TextMeshProUGUI>());
	}

	private void Update()
	{
		if(updateInUpdate) UpdateScore();
	}

	private void OnEnable()
	{
		UpdateScore();
	}

	public void UpdateScore()
	{
		//read the new score from the corresponding source
		int newScore = -1;
		switch (readSource)
		{
			case ScoreCategory.Current:
				newScore = readPoints ? GameManager.Points : GameManager.score;
				break;
			case ScoreCategory.First:
				newScore = PlayerPrefs.GetInt("Score 1", 0);
				break;
			case ScoreCategory.Second:
				newScore = PlayerPrefs.GetInt("Score 2", 0);
				break;
			case ScoreCategory.Third:
				newScore = PlayerPrefs.GetInt("Score 3", 0);
				break;
			case ScoreCategory.Other:
				newScore = PlayerPrefs.GetInt(otherSource, 0);
				break;
		}

		//update the text if it's different (and remember the new value)
		if (score != newScore)
		{
			score = newScore;
			text.Text = formatText.Replace("~", score.ToString());
		}
	}

	public void SetScore(int newScore)
	{
		score = newScore;
		text.Text = formatText.Replace("~", score.ToString());
	}
}

class TextComponent
{
	private bool isBasic;
	Text basicText;
	TextMeshProUGUI textMesh;

	public TextComponent (Text basic, TextMeshProUGUI mesh)
	{
		if (mesh)
		{
			textMesh = mesh;
			basicText = null;
			isBasic = false;
		}
		else
		{
			mesh = null;
			basicText = basic;
			isBasic = true;
		}
	}

	public string Text
	{
		get
		{
			if (isBasic)
			{
				return basicText.text;
			}
			else return textMesh.text;
		}
		set
		{
			if (isBasic)
			{
				basicText.text = value;
			}
			else textMesh.text = value;
		}
	}

}

public enum ScoreCategory { Current, First, Second, Third, Other, SetExternal };