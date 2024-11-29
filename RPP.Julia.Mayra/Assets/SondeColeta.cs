using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SondeColeta : MonoBehaviour
{
    public AudioSource Audio;
    void Start()
    {
        Audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Audio.Play();
        }
    }
}
