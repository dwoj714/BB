using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEventListener : MonoBehaviour
{
    private List<GameEvent> eventSubscriptions = new List<GameEvent>();

    public bool ListeningTo(GameEvent eventType)
	{
        return eventSubscriptions.Contains(eventType);
	}

    public abstract void Notify(GameEvent eventType);

    protected void SubscribeTo(GameEvent eventType)
	{
        GameEventManager.Subscribe(this);
        eventSubscriptions.Add(eventType);
	}

    protected void UnsubscribeFrom(GameEvent eventType)
	{
        eventSubscriptions.Remove(eventType);
        if(eventSubscriptions.Count == 0)
		{
            GameEventManager.Unsubscribe(this);
		}
	}
}
