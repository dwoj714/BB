using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

	[SerializeField] private List<FadingMultiElement> menuGroups;

	private int currentPage = 0;

	private bool inProgress = false;

	private void Start()
	{
		Page1Instant();
		GameManager.GameStarted += OnGameStart;
	}

	protected void OnGameStart(object o, EventArgs e)
	{
		Page1Instant();
	}

	private void Page1Instant()
	{
		foreach (FadingMultiElement e in menuGroups)
		{
			if (e == menuGroups[0])
				e.ShowInstant();
			else
				e.HideInstant();
		}

		currentPage = 0;

	}

	public void PageUp()
	{
		if (!inProgress) StartCoroutine(FadeUp());
	}

	public void PageDown()
	{
		if (!inProgress) StartCoroutine(FadeDown()) ;
	}

	private IEnumerator FadeUp()
	{
		inProgress = true;

		menuGroups[currentPage].sweepDirection = FadingElement.LEFT;
		StartCoroutine(menuGroups[currentPage].FadeOut());

		//increment current page if we're not on the last page, otherwise loop to the first page
		if (currentPage < menuGroups.Count - 1)
			currentPage++;
		else
			currentPage = 0;

		menuGroups[currentPage].sweepDirection = FadingElement.LEFT;
		yield return menuGroups[currentPage].FadeIn();

		inProgress = false;
	}

	private IEnumerator FadeDown()
	{
		inProgress = true;

		menuGroups[currentPage].sweepDirection = FadingElement.RIGHT;
		StartCoroutine(menuGroups[currentPage].FadeOut());

		//decrement current page if we're not on the first page, otherwise loop to the last page
		if (currentPage > 0)
			currentPage--;
		else
			currentPage = menuGroups.Count - 1;

		menuGroups[currentPage].sweepDirection = FadingElement.RIGHT;
		yield return menuGroups[currentPage].FadeIn();

		inProgress = false;
	}

}
