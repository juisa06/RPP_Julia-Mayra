using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject pauseMenu;
    public GameObject respawnMenu;
    public Text lifeText;
    public Text Scoretext;

    private bool isPaused = false;
    private int totalmedals = 0;
    public int LevelAtual = 1;
    private int lastMedalCount = 0;
    public int LifePlayer = 3;
    public int playerDamage = 1;
    public bool hasInvisiblePotion = false;
    public bool Estouinvisivel = false;
    public int Score;
    private bool isReturningToMenu = false; 
    public bool isPlayerDead = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isPlayerDead && !isReturningToMenu) // Verifica se não está voltando ao menu
        {
            // Remove a chamada de CheckForLevelUp() daqui

            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }
        }

        if (LifePlayer <= 0 && !isPlayerDead)
        {
            StartCoroutine(Die());
        }

        Scoretext.text = Score.ToString();
        UpdateLifeText();
    }
    
    public void CollectMedal() // Método a ser chamado ao coletar uma medalha
    {
        totalmedals++;
        CheckForLevelUp();  // Verifica se o nível deve ser incrementado após coletar a medalha
    }

    private void UpdateLifeText()
    {
        if (lifeText != null)
        {
            lifeText.text = LifePlayer.ToString();
        }
    }

    private string[] GetCurrentLevelCollectibles()
    {
        switch (LevelAtual)
        {
            case 1:
                return new string[] { "Animal1", "RareStone1", "PuzzlePiece1" };
            case 2:
                return new string[] { "Animal2", "RareStone2", "PuzzlePiece2" };
            default:
                return new string[] { };
        }
    }

    private void CheckForLevelUp()
    {
        if (totalmedals > lastMedalCount)
        {
            lastMedalCount = totalmedals;
            LevelAtual++;
            LoadNextLevel();
        }
    }

    private IEnumerator Die()
    {
        if (isPlayerDead) yield break;
        isPlayerDead = true;
        lastMedalCount = totalmedals;

        if (LifePlayer <= -10)
        {
            respawnMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        if (LifePlayer <= 0 && LifePlayer > -10)
        {
            yield return new WaitForSeconds(2f);

            respawnMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void Respawn()
    {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        playerTransform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
        
        LifePlayer = 3;
        isPlayerDead = false;
        respawnMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenuButon()
    {
        StartCoroutine(ReturnToMenu());
    }

    public IEnumerator ReturnToMenu()
    {
        isReturningToMenu = true;  // Define que está voltando ao menu
        SceneManager.LoadScene("MainMenu");  // Substitua com o nome da sua cena de menu principal
        ResetGame();
        lastMedalCount = totalmedals;  // Inicializa o lastMedalCount para igualar ao totalmedals no retorno ao menu
        respawnMenu.SetActive(false);
        yield return new WaitForSeconds(1);
        isReturningToMenu = false;  // Reseta a variável para evitar problemas futuros
    }

    private void LoadNextLevel()
    {
        if (LevelAtual < 5)
        {
            lastMedalCount = totalmedals;  // Ajusta o valor de lastMedalCount ao carregar o próximo nível
            string nextSceneName = "Level" + LevelAtual;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene("Info");
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void ResetGame()
    {
        // Reseta todas as variáveis globais do jogo para o estado inicial
        totalmedals = 0;
        LevelAtual = 1;
        lastMedalCount = totalmedals;  // Ajusta lastMedalCount para evitar incremento imediato
        LifePlayer = 3;
        playerDamage = 1;
        hasInvisiblePotion = false;
        Estouinvisivel = false;
        Score = 0;
        isPlayerDead = false;
        Time.timeScale = 1f;  // Retorna a escala de tempo ao normal caso estivesse pausado
    }
}
