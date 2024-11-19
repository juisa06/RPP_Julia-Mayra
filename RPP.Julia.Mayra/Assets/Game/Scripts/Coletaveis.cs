using UnityEngine;
using UnityEngine.SceneManagement;

public class PhaseManager2D : MonoBehaviour
{
    [Header("Configuração")]
    public string requiredItemName = "Chave"; // Nome do item necessário para liberar o portal.
    public GameObject portal; // Referência ao portal.
    private bool itemCollected = false;

    void Start()
    {
        // Certifique-se de que o portal começa desativado.
        if (portal != null)
        {
            portal.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o jogador coletou o item necessário.
        if (other.CompareTag("Item"))
        {
            if (other.name == requiredItemName)
            {
                CollectItem(other.gameObject);
            }
        }

        // Verifica se o jogador está interagindo com o portal.
        if (other.CompareTag("Portal") && itemCollected)
        {
            LoadNextPhase();
        }
    }

    void CollectItem(GameObject item)
    {
        Debug.Log("Item coletado: " + item.name);
        itemCollected = true;

        // Ativa o portal quando o item é coletado.
        if (portal != null)
        {
            portal.SetActive(true);
        }

        // Destrói o item para indicar que foi coletado.
        Destroy(item);
    }

    void LoadNextPhase()
    {
        // Obtém o nome da próxima cena com base na ordem das fases.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Todas as fases concluídas!");
            // Opcional: carregar uma cena de final ou reiniciar.
        }
    }
}