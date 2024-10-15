using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Medal : MonoBehaviour
{
    public int totalMedals;

    private void Start()
    {
        totalMedals = 0;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            MedalsOBSERVER.OnMedals(1);
            Destroy(gameObject);
        }
    }
}