using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;

public class FadingElement : MonoBehaviour
{

	[SerializeField] protected bool hideOnAwake = true;

	protected Graphic graphic;
	protected Color color, transparent;

	protected Vector2 origin;
	protected RectTransform rect;

	public float defaultFadeDuration = 1, defaultPauseDuration = 2;

	//angle in degrees indicating what direction to move the text, and distance
	public float sweepDirection = 0, sweepDistance = 100;

	public bool canFadeOut = true;

	public const float LEFT = 180f;
	public const float RIGHT = 0f;
	public const float UP = 90f;
	public const float DOWN = 270f;

	protected Vector2 FadeOutEndPoint
	{
		get
		{
			float x = Mathf.Cos(sweepDirection * Mathf.Deg2Rad) * sweepDistance;
			float y = Mathf.Sin(sweepDirection * Mathf.Deg2Rad) * sweepDistance;
			
			return origin + (Vector2.right * x + Vector2.up * y);
		}
	}
	protected Vector2 FadeInStartPoint
	{
		get
		{
			float x = Mathf.Cos(sweepDirection * Mathf.Deg2Rad) * sweepDistance;
			float y = Mathf.Sin(sweepDirection * Mathf.Deg2Rad) * sweepDistance;

			return origin - (Vector2.right * x + Vector2.up * y);
		}
	}

	protected virtual void Awake()
	{
		rect = transform as RectTransform;
		origin = rect.anchoredPosition;

		graphic = GetComponent<Graphic>();
		color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, graphic.color.a);
		transparent = new Color(color.r, color.g, color.b, 0);

		if(hideOnAwake)
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

	public virtual IEnumerator FadeIn(float fadeDuration = -1)
	{

		if (fadeDuration < 0) fadeDuration = defaultFadeDuration;

		graphic.color = transparent;

		float timer = 0;
		float progress = 0;

		//interpolate position and color to origin point and the graphic's normal color
		while (timer < fadeDuration)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, fadeDuration);
			progress = timer / fadeDuration;

			graphic.color = Color.Lerp(transparent, color, progress);
			rect.anchoredPosition = Vector2.Lerp(FadeInStartPoint, origin, Mathf.Sqrt(progress));

			yield return null;
		}
	}

	public virtual IEnumerator FadeOut(float fadeDuration = -1)
	{

		if (fadeDuration < 0) fadeDuration = defaultFadeDuration;

		graphic.color = color;

		float timer = 0;
		float progress = 0;

		//interpolate position and color to fade-out point and fully transparent color
		while (timer < fadeDuration)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, fadeDuration);
			progress = timer / fadeDuration;

			graphic.color = Color.Lerp(color, transparent, progress);
			rect.anchoredPosition = Vector2.Lerp(origin, FadeOutEndPoint, Mathf.Pow(progress, 2));

			yield return null;
		}
	}

	public virtual IEnumerator FadeSequence(float pauseDuration, float fadeDuration)
	{
		yield return FadeIn(fadeDuration);

		//Pause for the specified time while text is fully visible
		yield return new WaitForSeconds(pauseDuration);

		//Remain paused until canFadeOut is set to true (true by default)
		while (!canFadeOut) yield return null;

		yield return FadeOut(fadeDuration);
		
	}

	public IEnumerator DelayedFadeSequence(float delay)
	{
		yield return new WaitForSeconds(delay);
		yield return FadeSequence();
	}

	public virtual void HideInstant()
	{
		StopAllCoroutines();
		graphic.color = transparent;
	}

	public virtual void ShowInstant()
	{
		StopAllCoroutines();
		graphic.color = color;
		rect.anchoredPosition = origin;
	}

}
