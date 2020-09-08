using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialSequencer : GameEventListener
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

        SubscribeTo(GameEvent.GameStart);
    }

    void OnGameStart()
	{
        StartCoroutine(PlayTutorialSequence());
        SubscribeTo(GameEvent.ShotFired);
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

    public override void Notify(GameEvent eventType)
	{
		switch (eventType)
		{
            case GameEvent.ShotFired:
                shotFired = true;
                break;
            case GameEvent.GameStart:
                OnGameStart();
                break;
		}
	}

}
