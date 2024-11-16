using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemType; // Pode ser "Animal", "RareStone", "PuzzlePiece"
    public int itemIndex; // Índice do item coletável (0-5)

    public AudioSource Colect;

    private void Start()
    {
        Colect = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Colect.Play();
            // Encontra o CollectibleManager
            CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
            if (collectibleManager == null)
            {
                Debug.LogError("CollectibleManager não encontrado!");
                return;
            }

            // Coleta o item
            CollectItem(collectibleManager);
            Debug.Log($"{itemType} Coletado no índice: {itemIndex}");

            // Destrói o item após a coleta
            Destroy(gameObject,0.2f);
        }
    }

    private void CollectItem(CollectibleManager collectibleManager)
    {
        switch (itemType)
        {
            case "RareStone":
                collectibleManager.CollectRareStone(itemIndex);
                break;
            case "PuzzlePiece":
                collectibleManager.CollectPuzzlePiece(itemIndex);
                break;
            case "Animal":
                collectibleManager.CollectRareAnimal(itemIndex);
                break;
            default:
                Debug.LogWarning($"Tipo de item desconhecido: {itemType}");
                break;
        }
    }
}