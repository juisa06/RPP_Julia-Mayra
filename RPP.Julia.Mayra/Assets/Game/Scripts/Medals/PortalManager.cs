using System;
using UnityEngine;

public class PortalUnlock2D : MonoBehaviour
{
    // Referência para o GameObject do portal
    public GameObject portal;

    // Referência ao item que o jogador deve coletar
    public GameObject itemToCollect;

    // Função chamada quando o jogador colide com o item
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu com o item é o jogador
        if (other.CompareTag("Player"))
        {
            // Desativa o item após a coleta
            //itemToCollect.SetActive(false);
            //Destroy(itemToCollect, 0.2f);
            // Ativa o portal, incluindo o seu colisor, para liberar a passagem
            if (portal != null)
            {
                // Ativa o portal e seu colisor
                portal.SetActive(true);

                // Certifique-se de que o colisor 2D do portal também seja ativado
                Collider2D portalCollider = portal.GetComponent<Collider2D>();
                if (portalCollider != null)
                {
                    portalCollider.enabled = true;
                }
            }

            // Você pode adicionar efeitos adicionais aqui, como animações ou sons de desbloqueio
        }
    }
}