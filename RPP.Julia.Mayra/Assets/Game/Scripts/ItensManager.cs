using System.Collections.Generic;
using UnityEngine;

public class ItensManager : MonoBehaviour
{
    // Dicionário para armazenar os ícones das UI, com o nome do item como chave
    private Dictionary<string, GameObject> itemUIMap;

    // Método Start para inicializar o dicionário
    void Start()
    {
        itemUIMap = new Dictionary<string, GameObject>
        {
            { "Pena1", pena1UI },
            { "Pena2", pena2UI },
            { "Pena3", pena3UI },
            { "Pena4", pena4UI },
            { "Medalha1", medalha1UI },
            { "Medalha2", medalha2UI },
            { "Medalha3", medalha3UI },
            { "Medalha4", medalha4UI },
            { "Flor1", flor1UI },
            { "Flor2", flor2UI },
            { "Flor3", flor3UI }
        };
    }

    public void ActivateItemByName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("Nome do item é inválido.");
            return;
        }

        if (itemUIMap.ContainsKey(itemName))
        {
            itemUIMap[itemName].SetActive(true); // Ativa o item na UI
            DestroyItem(itemName); // Destroi o item coletado
        }
        else
        {
            Debug.LogWarning($"Item desconhecido: {itemName}");
        }
    }
    private void DestroyItem(string itemName)
    {
        GameObject item = GameObject.Find(itemName); // Encontra o objeto pelo nome
        if (item != null)
        {
            Destroy(item,0.2f); // Destroi o objeto coletado
        }
        else
        {
            Debug.LogWarning($"Objeto com o nome {itemName} não foi encontrado para destruição.");
        }
    }

    // Referências às imagens no painel
    public GameObject pena1UI;
    public GameObject pena2UI;
    public GameObject pena3UI;
    public GameObject pena4UI;

    public GameObject medalha1UI;
    public GameObject medalha2UI;
    public GameObject medalha3UI;
    public GameObject medalha4UI;

    public GameObject flor1UI;
    public GameObject flor2UI;
    public GameObject flor3UI;
}