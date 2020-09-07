using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingMultiElement : FadingElement
{
	[SerializeField] private Graphic[] subGraphics;
	private Color[] subColors;
	private Color[] subTransparents;

	protected override void Start()
	{
		base.Start();

		subColors = new Color[subGraphics.Length];
		subTransparents = new Color[subGraphics.Length];

		for(int i = 0; i < subGraphics.Length; i++)
		{
			subColors[i] = subGraphics[i].color;

			subTransparents[i].r = subColors[i].r;
			subTransparents[i].g = subColors[i].g;
			subTransparents[i].b = subColors[i].b;
			subTransparents[i].a = 0;

			subGraphics[i].color = subTransparents[i];
		}

	}

	public override IEnumerator FadeSequence(float pauseDuration, float fadeDuration)
	{
		Debug.Log("Ya Boy");
		graphic.color = transparent;

		float x = Mathf.Cos(sweepDirection * Mathf.Deg2Rad) * sweepDistance;
		float y = Mathf.Sin(sweepDirection * Mathf.Deg2Rad) * sweepDistance;
		Vector2 delta = new Vector2(x, y);

		Vector2 startPos = origin - delta;
		Vector2 endPos = origin + delta;

		float progress = 0;
		float timer = 0;

		//start color fade-in coroutine for sub-elements
		StartCoroutine(FadeInSubObjects(fadeDuration));

		//interpolate position and color to origin point and the graphic's normal color
		while (timer < fadeDuration)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, fadeDuration);
			progress = timer / fadeDuration;

			graphic.color = Color.Lerp(transparent, color, progress);
			rect.anchoredPosition = Vector2.Lerp(startPos, origin, Mathf.Sqrt(progress));

			yield return null;
		}

		//Pause for the specified time while text is fully visible
		yield return new WaitForSeconds(pauseDuration);

		//Remain paused until canFadeOut is set to true (true by default)
		while (!canFadeOut) yield return null;

		//start color fade-out coroutine for sub-elements
		StartCoroutine(FadeOutSubObjects(fadeDuration));

		//interpolate position and color to fade-out point and fully transparent color
		timer = 0;
		while (timer < fadeDuration)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, fadeDuration);
			progress = timer / fadeDuration;

			graphic.color = Color.Lerp(color, transparent, progress);
			rect.anchoredPosition = Vector2.Lerp(origin, endPos, Mathf.Pow(progress, 2));

			yield return null;
		}
	}

	//rewrite this so it handles all subobjects in one go instead of needing to be called once for each object
	public IEnumerator FadeInSubObjects(float fadeDuration)
	{
		float progress;
		float timer = 0;

		//interpolate transparent color to the graphic's normal color
		while (timer < fadeDuration)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, fadeDuration);
			progress = timer / fadeDuration;

			for(int i = 0; i < subGraphics.Length; i++)
			{
				subGraphics[i].color = Color.Lerp(subTransparents[i], subColors[i], progress);
			}

			yield return null;
		}
	}

	public IEnumerator FadeOutSubObjects(float fadeDuration)
	{
		float progress;
		float timer = 0;

		//interpolate graphic's normal color to fully transparent color
		while (timer < fadeDuration)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, fadeDuration);
			progress = timer / fadeDuration;

			for (int i = 0; i < subGraphics.Length; i++)
			{
				subGraphics[i].color = Color.Lerp(subColors[i], subTransparents[i], progress);
			}

			yield return null;
		}
	}

}