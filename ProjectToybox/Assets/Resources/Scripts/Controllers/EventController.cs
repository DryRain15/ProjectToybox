using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class EventParameter
{
    public Dictionary<string, object> Param;

    public EventParameter(params KeyValuePair<string, object>[] dict)
    {
        Param = new Dictionary<string, object>();
        foreach (var pair in dict)
        {
            Param.Add(pair.Key, pair.Value);
        }
    }

    public object this[string key]
    {
        get => Param[key];
        set => Param[key] = value;
    }

    [CanBeNull]
    public T Get<T>(string key)
    {
        return this[key] is T ? (T)this[key] : default;
    }
}

public class EventController : MonoBehaviour
{
    public static EventController Instance;
    
    public Dictionary<string, UnityEvent<EventParameter>> Events;

    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
         else Instance = this;
        
        Events = new Dictionary<string, UnityEvent<EventParameter>>();
    }

    public void EventCall(string eventName, EventParameter param)
    {
        if (!Events.ContainsKey(eventName))
            return;
        
        Events[eventName].Invoke(param);
    }

    public void Subscribe(string eventName, UnityAction<EventParameter> action)
    {
        if (!Events.ContainsKey(eventName))
            Events.Add(eventName, new UnityEvent<EventParameter>());

        Events[eventName].AddListener(action);
    }

    public void Unsubscribe(string eventName, UnityAction<EventParameter> action)
    {
        if (Events.ContainsKey(eventName))
        {
            Events[eventName].RemoveListener(action);
            Events.Remove(eventName);
        }
    }

}
