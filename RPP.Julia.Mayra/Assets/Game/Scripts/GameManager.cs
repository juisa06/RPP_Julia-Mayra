using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject pauseMenu;
    public GameObject respawnMenu;  // Novo painel para respawn ou menu
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
        if (!isPlayerDead)
        {
            totalmedals = MedalManager.MedalsCount;
            CheckForLevelUp();

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

        yield return new WaitForSeconds(2f);

        // Exibe o menu de respawn com opções para o jogador
        respawnMenu.SetActive(true);
        Time.timeScale = 0f;  // Pausar o jogo enquanto o jogador escolhe
    }

    public void Respawn()
    {
        // Restaurar o jogador para o último checkpoint salvo
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        playerTransform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
        
        LifePlayer = 3;  // Restaurar a vida do jogador
        isPlayerDead = false;
        respawnMenu.SetActive(false);
        Time.timeScale = 1f;  // Retomar o jogo
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;  // Retomar o tempo antes de carregar o menu
        SceneManager.LoadScene("MainMenu");  // Substitua com o nome da sua cena de menu principal
    }

    private void LoadNextLevel()
    {
        if (LevelAtual < 5)
        {
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
}
