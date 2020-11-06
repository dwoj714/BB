using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingMultiElement : FadingElement
{
	[SerializeField] private List<Graphic> subGraphics;
	private Color[] subColors;
	private Color[] subTransparents;

	protected override void Awake()
	{
		base.Awake();

		subColors = new Color[subGraphics.Count];
		subTransparents = new Color[subGraphics.Count];

		for(int i = 0; i < subGraphics.Count; i++)
		{
			subColors[i] = subGraphics[i].color;

			subTransparents[i].r = subColors[i].r;
			subTransparents[i].g = subColors[i].g;
			subTransparents[i].b = subColors[i].b;
			subTransparents[i].a = 0;

			if(hideOnAwake)
				subGraphics[i].color = subTransparents[i];
		}

	}

	public override IEnumerator FadeSequence(float pauseDuration, float fadeDuration)
	{
		//start color fade-in coroutine for sub-elements
		StartCoroutine(FadeInSubObjects(fadeDuration));

		//Yield for the execution of the main Fade-in coroutine
		yield return FadeIn(fadeDuration);

		//Pause for the specified time while text is fully visible
		yield return new WaitForSeconds(pauseDuration);

		//Remain paused until canFadeOut is set to true (true by default)
		while (!canFadeOut) yield return null;

		//start color fade-out coroutine for sub-elements
		StartCoroutine(FadeOutSubObjects(fadeDuration));

		//Yield for the execution of the main Fade-out coroutine
		yield return FadeOut(fadeDuration);
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

			for(int i = 0; i < subGraphics.Count; i++)
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

			for (int i = 0; i < subGraphics.Count; i++)
			{
				subGraphics[i].color = Color.Lerp(subColors[i], subTransparents[i], progress);
			}

			yield return null;
		}
	}

	public override IEnumerator FadeIn(float fadeDuration = -1)
	{
		if (fadeDuration < 0) fadeDuration = defaultFadeDuration;
		StartCoroutine(FadeInSubObjects(fadeDuration));
		yield return base.FadeIn(fadeDuration);
	}

	public override IEnumerator FadeOut(float fadeDuration = -1)
	{
		if (fadeDuration < 0) fadeDuration = defaultFadeDuration;
		StartCoroutine(FadeOutSubObjects(fadeDuration));
		yield return base.FadeOut(fadeDuration);
	}

	public override void HideInstant()
	{
		base.HideInstant();
		for(int i = 0; i < subGraphics.Count; i++)
		{
			subGraphics[i].color = subTransparents[i];
		}
	}

	public override void ShowInstant()
	{
		base.ShowInstant();
		for (int i = 0; i < subGraphics.Count; i++)
		{
			subGraphics[i].color = subColors[i];
		}
	}

}