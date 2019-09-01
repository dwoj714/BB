using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressIndicatorController : MonoBehaviour
{
	[SerializeField] private DifficultyManager man;
	[SerializeField] private RectTransform progressLine, shadowLine;

	[SerializeField] private Image massIcon, healthIcon;
	[SerializeField] private TextMeshProUGUI massMult, healthMult;

	private Rect min, max;
	private bool updated = false;
	private float prevProgress = 1;

    void Start()
    {
		min = progressLine.rect;
		max = shadowLine.rect;
    }

    void Update()
    {
		float width = man.WaveProgress * (max.width - min.width) + min.width;
		progressLine.offsetMax = Vector2.Lerp(progressLine.offsetMax, Vector2.up * progressLine.offsetMax.y + Vector2.right * width, 0.2f);

		if(!updated || prevProgress > man.WaveProgress)
		{
			healthMult.text = BombController.healthMod + "x";
			massMult.text = BombController.massMod + "x";
			updated = true;
		}
		prevProgress = man.WaveProgress;
	}

	public void ResetProgress()
	{
		progressLine.offsetMax = Vector2.up * progressLine.offsetMax.y + Vector2.right * min.width;
		updated = false;
	}

}
