using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private RectTransform button1, button2;
	private RectTransform rect;

	//offsets corresponding to hidden/visible states of the buttons
	//[SerializeField] private Vector2 hidden1, hidden2, visible1, visible2;
	[SerializeField] private Vector2 offset1, offset2;
	[SerializeField] private Vector2 sizeOffset;

	private Vector2 origin1, origin2;
	private Vector2 originSize;

	[SerializeField] private Text text;
	[SerializeField] private string showText, hideText;

#pragma warning restore 0649

	private void Start()
	{
		origin1 = button1.anchoredPosition;
		origin2 = button2.anchoredPosition;
		
		rect = transform as RectTransform;
		originSize = rect.sizeDelta;
	}

	private bool hidden = true;

	public float transitionTime = 0.4f;
	
	public void ToggleConfirm()
	{
		StopAllCoroutines();
		if (hidden)
		{
			StartCoroutine(Show());
		}
		else
		{
			StartCoroutine(Hide());
		}
	}

	IEnumerator Show()
	{
		
		hidden = false;

		text.text = showText;

		Vector2 origin1 = button1.anchoredPosition;
		Vector2 origin2 = button2.anchoredPosition;
		Vector2 originSize = rect.sizeDelta;

		Vector2 dest1 = this.origin1 + offset1;
		Vector2 dest2 = this.origin2 + offset2;
		Vector2 destSize = this.originSize + sizeOffset;

		float timer = 0;
		do
		{
			timer += Time.unscaledDeltaTime;
			if (timer > transitionTime) timer = transitionTime;

			float progress = timer / transitionTime;

			button1.anchoredPosition = origin1 + (dest1 - origin1) * progress;
			button2.anchoredPosition = origin2 + (dest2 - origin2) * progress;

			rect.sizeDelta = originSize + (destSize - originSize) * progress;

			yield return null;
		}
		while (timer < transitionTime);
	}

	IEnumerator Hide()
	{
		hidden = true;

		text.text = hideText;

		Vector2 origin1 = button1.anchoredPosition;
		Vector2 origin2 = button2.anchoredPosition;
		Vector2 originSize = rect.sizeDelta;

		Vector2 dest1 = this.origin1;
		Vector2 dest2 = this.origin2;
		Vector2 destSize = this.originSize;

		float timer = 0;
		do
		{
			timer += Time.unscaledDeltaTime;
			if (timer > transitionTime) timer = transitionTime;

			float progress = timer / transitionTime;

			button1.anchoredPosition = origin1 + (dest1 - origin1) * progress;
			button2.anchoredPosition = origin2 + (dest2 - origin2) * progress;

			rect.sizeDelta = originSize + (destSize - originSize) * progress;

			yield return null;
		}
		while (timer < transitionTime);
	}


	public void HideInstant()
	{
		hidden = true;
		button1.anchoredPosition = origin1;
		button2.anchoredPosition = origin2;
		rect.sizeDelta = originSize;
		text.text = hideText;
	}

	private void OnDisable()
	{
		HideInstant();
	}

}
