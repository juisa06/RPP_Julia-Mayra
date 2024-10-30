using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIncrase : MonoBehaviour
{
    public AudioSource Audio;

    private void Start()
    {
        Audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.gameObject.tag == "Player")
        {
            Destroy(gameObject, 0.2f);
            GameManager.Instance.playerDamage++;
            Audio.Play();
        }
    }
}