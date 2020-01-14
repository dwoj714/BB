using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
	[SerializeField] private RectTransform button1, button2;

	//offsets corresponding to hidden/visible states of the buttons
	//[SerializeField] private Vector2 hidden1, hidden2, visible1, visible2;
	[SerializeField] private Vector2 offset1, offset2;
	private Vector2 origin1, origin2;

	[SerializeField] private Text text;
	[SerializeField] private string showText, hideText;

	private void Start()
	{
		origin1 = button1.anchoredPosition;
		origin2 = button2.anchoredPosition;
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

		Vector2 dest1 = this.origin1 + offset1;
		Vector2 dest2 = this.origin2 + offset2;

		float timer = 0;
		do
		{
			timer += Time.unscaledDeltaTime;
			if (timer > transitionTime) timer = transitionTime;

			button1.anchoredPosition = origin1 + (dest1 - origin1) * (timer / transitionTime);
			button2.anchoredPosition = origin2 + (dest2 - origin2) * (timer / transitionTime);

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

		Vector2 dest1 = this.origin1;
		Vector2 dest2 = this.origin2;

		float timer = 0;
		do
		{
			timer += Time.unscaledDeltaTime;
			if (timer > transitionTime) timer = transitionTime;

			button1.anchoredPosition = origin1 + (dest1 - origin1) * (timer / transitionTime);
			button2.anchoredPosition = origin2 + (dest2 - origin2) * (timer / transitionTime);

			yield return null;
		}
		while (timer < transitionTime);
	}


	public void HideInstant()
	{
		hidden = true;
		button1.anchoredPosition = origin1;
		button2.anchoredPosition = origin2;
		text.text = hideText;
	}

	private void OnDisable()
	{
		HideInstant();
	}

}
