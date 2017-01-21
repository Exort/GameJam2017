using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager:BaseSingleton<EventManager>
{
    private List<EventListener> listeners = new List<EventListener>();
    public void Register(EventListener theClass)
    {
        if (!listeners.Contains(theClass))
        {
            listeners.Add(theClass);
        }
    }
    public void Unregister(EventListener theClass)
    {
        listeners.Remove(theClass);
    }
    public void SendEvent(EventType theType, object param)
    {
        foreach (EventListener lst in listeners)
        {
            lst.OnMessage(theType, param);
        }
    }

}
