using System.Collections;
using System;
using UnityEngine;

public class TutorialSequencer : MonoBehaviour
{
    public static TutorialSequencer main;

    [SerializeField] private GameObject container;

    private bool inProgress = false;
    public bool InProgress
	{
        get;
	}

    private bool shotFired = false;

    [SerializeField] private FadingElement[] elements;

    void Start()
    {
        container.SetActive(true);
        if (!main) main = this;
        else Debug.LogWarning("More than one TutorialSequencer initialized!");
        GameManager.GameStarted += OnGameStart;
    }

    void OnGameStart(object o, EventArgs e)
	{
        Debug.Log("Event received from " + (o as MonoBehaviour).gameObject.name);
        StartCoroutine(PlayTutorialSequence());
        LauncherController.ShotFired += OnShotFired;
    }

    private IEnumerator PlayTutorialSequence()
	{
        inProgress = true;

        StartCoroutine(elements[3].DelayedFadeSequence(DifficultyManager.main.interval * 0.8f));

        shotFired = false;

        StartCoroutine(EnableFadeOutOnShotFired(elements[0]));
        StartCoroutine(EnableFadeOutOnShotFired(elements[1]));

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(elements[0].FadeSequence(3.5f));
        yield return new WaitForSeconds(2);
        StartCoroutine(elements[1].FadeSequence(2));

        yield return new WaitForSeconds(4);
        yield return elements[2].FadeSequence();

        yield return new WaitForSeconds(1);
        StartCoroutine(elements[4].FadeSequence());

        inProgress = false;
	}

    private IEnumerator EnableFadeOutOnShotFired(FadingElement e)
	{
        shotFired = false;
        e.canFadeOut = false;
        while (!shotFired) yield return null;
        e.canFadeOut = true;
	}

    protected void OnShotFired(object o, EventArgs e)
	{
        shotFired = true;
	}

}
