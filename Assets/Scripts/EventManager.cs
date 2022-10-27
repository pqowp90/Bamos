using System.Collections.Generic;
using System;

public class EventManager
{


    private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();
    private static Dictionary<string, Action<int>> eventDictionary2 = new Dictionary<string, Action<int>>();


    /// <summary>
    /// EventManager.FunctionName("KeyName" , Action)
    /// </summary>
    /// <param name = "eventName">오호호</param>
    public static void StartListening(string eventName, Action listener)
    {
        Action thisEvent;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary.Add(eventName, listener);
        }
    }

    public static void StopListening(string eventName, Action listener)
    {
        Action thisEvent;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary.Remove(eventName);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        Action thisEvent;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke();
        }
    }

    public static void StartListening(string eventName, Action<int> listener, int value)
    {
        Action<int> thisEvent;
        if (eventDictionary2.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            eventDictionary2[eventName] = thisEvent;
        }
        else
        {
            eventDictionary2.Add(eventName, listener);
        }
    }

    public static void StopListening(string eventName, Action<int> listener, int value)
    {
        Action<int> thisEvent;
        if (eventDictionary2.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            eventDictionary2[eventName] = thisEvent;
        }
        else
        {
            eventDictionary2.Remove(eventName);
        }
    }

    public static void TriggerEvent(string eventName, int value)
    {
        Action<int> thisEvent;
        if (eventDictionary2.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke(value);
        }
    }


}
