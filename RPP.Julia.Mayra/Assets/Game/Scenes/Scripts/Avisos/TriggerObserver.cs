using System;
using UnityEngine;

public static class TriggerObserver
{
    public static event Action<string> OnTriggerEnterEvent;

    public static void TriggerEnter(string triggerName)
    {
        OnTriggerEnterEvent?.Invoke(triggerName);
    }
}