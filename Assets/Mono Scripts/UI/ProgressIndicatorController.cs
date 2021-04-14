using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressIndicatorController : MonoBehaviour
{
#pragma warning disable 0649

	[SerializeField] private DifficultyManager man;
	[SerializeField] private RectTransform progressLine, shadowLine;

	[SerializeField] private Image massIcon, healthIcon;
	[SerializeField] private Text massMult, healthMult;

	[SerializeField] private float scaleScale = 1.5f;
	[SerializeField] private Color flashColor = Color.white;
	[SerializeField] private float resetLerp = 6;

#pragma warning restore 0649

	private Color massIC, healthIC, massTC, healthTC;
	private Vector3 massScale, healthScale;

	private Rect min, max;
	private bool updated = false;
	private float prevProgress = 1;

    void Start()
    {
		min = progressLine.rect;
		max = shadowLine.rect;

		massIC = massIcon.color;
		healthIC = healthIcon.color;
		massTC = massMult.color;
		healthTC = healthMult.color;

		massScale = massIcon.transform.localScale;
		healthScale = healthIcon.transform.localScale;

    }

    void Update()
    {
		if (GameManager.gameInProgress)
		{
			float width = GameplayController.main.WaveProgress() * (max.width - min.width) + min.width;
			progressLine.offsetMax = Vector2.Lerp(progressLine.offsetMax, Vector2.up * progressLine.offsetMax.y + Vector2.right * width, 0.2f);
			prevProgress = GameplayController.main.WaveProgress();

			if (!updated || prevProgress > GameplayController.main.WaveProgress())
			{
				healthMult.text = BombController.healthMod + "x";
				massMult.text = BombController.massMod + "x";
				updated = true;
			}
		}
	}

	public void ResetProgress()
	{
		progressLine.offsetMax = Vector2.up * progressLine.offsetMax.y + Vector2.right * min.width;
		updated = false;
	}

	public IEnumerator FlashHealth()
	{
		healthMult.text = BombController.healthMod + "x";
		healthIcon.transform.localScale *= scaleScale;
		healthMult.color = healthIcon.color = flashColor;

		do
		{
			healthIcon.transform.localScale = Vector3.Lerp(healthIcon.transform.localScale, healthScale, resetLerp * Time.deltaTime);
			healthIcon.color = Color.Lerp(healthIcon.color, healthIC, resetLerp * Time.deltaTime);
			healthMult.color = Color.Lerp(healthMult.color, healthTC, resetLerp * Time.deltaTime);
			yield return null;
		} while (!Mathf.Approximately(healthIcon.transform.localScale.sqrMagnitude, healthScale.sqrMagnitude));

	}

	public IEnumerator FlashMass()
	{
		massMult.text = BombController.massMod + "x";
		massIcon.transform.localScale *= scaleScale;
		massMult.color = massIcon.color = flashColor;

		do
		{
			massIcon.transform.localScale = Vector3.Lerp(massIcon.transform.localScale, massScale, resetLerp * Time.deltaTime);
			massIcon.color = Color.Lerp(massIcon.color, massIC, resetLerp * Time.deltaTime);
			massMult.color = Color.Lerp(massMult.color, massTC, resetLerp * Time.deltaTime);
			yield return null;
		} while (!Mathf.Approximately(massIcon.transform.localScale.sqrMagnitude, massScale.sqrMagnitude));
	}

}
