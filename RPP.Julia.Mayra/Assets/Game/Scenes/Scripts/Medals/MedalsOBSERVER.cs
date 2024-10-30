using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MedalsOBSERVER
{

    public static event Action<int> Medals;

    public static void OnMedals(int obj)
    {
        Medals?.Invoke(obj);
    }
}