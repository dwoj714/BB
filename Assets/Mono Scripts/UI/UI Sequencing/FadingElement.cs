using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;

public class FadingElement : MonoBehaviour
{

	protected Graphic graphic;
	protected Color color, transparent;

	protected Vector2 origin;
	protected RectTransform rect;

	public float defaultFadeDuration = 1, defaultPauseDuration = 2;

	//angle in degrees indicating what direction to move the text, and distance
	public float sweepDirection = 0, sweepDistance = 100;

	public bool canFadeOut = true;

	protected virtual void Start()
	{
		rect = transform as RectTransform;
		origin = rect.anchoredPosition;

		graphic = GetComponent<Graphic>();
		color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, graphic.color.a);
		transparent = new Color(color.r, color.g, color.b, 0);
		graphic.color = transparent;

	}

	public virtual IEnumerator FadeSequence()
	{
		yield return FadeSequence(defaultPauseDuration, defaultFadeDuration);
	}

	public virtual IEnumerator FadeSequence(float pauseDuration)
	{
		yield return FadeSequence(pauseDuration, defaultFadeDuration);
	}

	public virtual IEnumerator FadeSequence(float pauseDuration, float fadeDuration)
	{
		graphic.color = transparent;

		float x = Mathf.Cos(sweepDirection * Mathf.Deg2Rad) * sweepDistance;
		float y = Mathf.Sin(sweepDirection * Mathf.Deg2Rad) * sweepDistance;
		Vector2 delta = new Vector2(x, y);

		Vector2 startPos = origin - delta;
		Vector2 endPos = origin + delta;

		float progress = 0;
		float timer = 0;

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

	public IEnumerator DelayedFadeSequence(float delay)
	{
		yield return new WaitForSeconds(delay);
		yield return FadeSequence();
	}
}
