using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject pauseMenu;
    public Text lifeText;  // Variável para o texto da vida

    private bool isPaused = false;
    private int totalmedals = 0;
    public int LevelAtual = 1;
    private int lastMedalCount = 0;
    public int LifePlayer = 3;
    public int playerDamage = 1;
    public bool hasInvisiblePotion = false;
    public bool Estouinvisivel = false;

    private bool isPlayerDead = false;

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

        if (LifePlayer <= 0)
        {
            Die();
        }

        UpdateLifeText();  // Atualizar o texto da vida na tela
    }

    // Atualiza o texto de vidas na tela
    private void UpdateLifeText()
    {
        if (lifeText != null)
        {
            lifeText.text = "Vida: " + LifePlayer.ToString();
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
            // Continue para os outros níveis
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

    private void Die()
    {
        if (isPlayerDead) return;
        isPlayerDead = true;
        lastMedalCount = totalmedals;

        // Mover o jogador para o último checkpoint salvo
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        playerTransform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
        TogglePause();
        LifePlayer = 3; // Restaurar a vida do jogador
        isPlayerDead = false;
    }

    private void LoadNextLevel()
    {
        string nextSceneName = "Level" + LevelAtual;
        SceneManager.LoadScene(nextSceneName);
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