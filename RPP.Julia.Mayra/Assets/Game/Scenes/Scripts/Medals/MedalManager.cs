using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MedalManager : MonoBehaviour
{
    public static int MedalsCount = 0;
    public void ADDMedals(int i)
    {
        MedalsCount += i;
    }

    private void OnEnable()
    {
        MedalsOBSERVER.Medals += ADDMedals;
    }

    private void OnDisable()
    {
        MedalsOBSERVER.Medals -= ADDMedals;
    }
}
