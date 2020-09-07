using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class GameEventManager
{
	private static List<GameEventListener> listeners = new List<GameEventListener>();

    public static void EventTriggered(GameEvent eventType)
	{
		foreach(GameEventListener listener in listeners)
		{
			if (listener.ListeningTo(eventType))
			{
				listener.Notify(eventType);
			}
		}
	}

	public static void Subscribe(GameEventListener listener)
	{
		if (!listeners.Contains(listener))
		{
			listeners.Add(listener);
		}
	}
	public static void Unsubscribe(GameEventListener listener)
	{
		if (listeners.Contains(listener))
		{
			listeners.Remove(listener);
		}
	}


}

public enum GameEvent
{
    ShotFired,
    BombDetonated,
	GameStart,
	GameEnd

}
