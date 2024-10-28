using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Telas do Menu")]
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;

    void Start()
    {
        ShowMainMenu();
    }

    // Exibe o Menu Principal e oculta as outras telas
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        instructionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    // Exibe a tela de Instruções e oculta as outras
    public void ShowInstructions()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    // Exibe a tela de Créditos e oculta as outras
    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    // Método para o botão de Jogar que inicia o jogo
    public void PlayGame()
    {
        // Carrega a primeira cena do jogo
        SceneManager.LoadScene("Level1"); // Substitua pelo nome da sua cena
    }

    // Método para sair do jogo (não funcionará no editor)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Jogo fechado"); // Somente para teste no editor
    }
}