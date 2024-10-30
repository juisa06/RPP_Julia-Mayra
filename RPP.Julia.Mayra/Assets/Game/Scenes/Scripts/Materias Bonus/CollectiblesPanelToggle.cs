using UnityEngine;

public class CollectiblesPanelToggle : MonoBehaviour
{
    public GameObject collectiblesPanel;  // Referência ao painel de colecionáveis
    private bool isPanelOpen = false;      // Estado atual do painel

    private void Start()
    {
        collectiblesPanel.SetActive(false); // Inicia o painel invisível
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;

        // Alterna a visibilidade do painel
        collectiblesPanel.SetActive(isPanelOpen);

        // Pausa ou retoma o jogo
        Time.timeScale = isPanelOpen ? 0f : 1f;
    }
}