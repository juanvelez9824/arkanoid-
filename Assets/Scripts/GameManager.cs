using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private KeyCode pauseKey = KeyCode.P;
    [SerializeField] private string[] levelScenes;
    
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action OnGameOver;
    private int currentLevel;
    public event Action OnGameRestart;
     public event Action<int> OnLevelChanged;
    
    private int currentScore;
    private int currentLives;
    private bool isGameOver;

    private void Awake()
    {
        SetupInstance();
    }

    private void Update()
    {
        if (!isGameOver && Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    private void SetupInstance()
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

    public void InitializeGame()
    {
        Debug.Log("Initializing game");
        currentScore = 0;
        currentLives = startingLives;
        isGameOver = false;
        Time.timeScale = 1f;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(currentScore);
            UIManager.Instance.UpdateLivesUI(currentLives);
        }
        
        OnScoreChanged?.Invoke(currentScore);
        OnLivesChanged?.Invoke(currentLives);
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;

        currentScore += points;
        UIManager.Instance?.UpdateScoreUI(currentScore);
        OnScoreChanged?.Invoke(currentScore);
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        UIManager.Instance?.UpdateLivesUI(currentLives);
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        OnGameOver?.Invoke();
        UIManager.Instance?.ShowGameOver(currentScore);
        Debug.Log($"Game Over! Final Score: {currentScore}");
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game");
        OnGameRestart?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading main menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void TogglePause()
    {
        bool isPaused = Time.timeScale == 0f;
        Time.timeScale = isPaused ? 1f : 0f;
        
        if (UIManager.Instance != null)
        {
            if (isPaused)
            {
                UIManager.Instance.HidePauseMenu();
            }
            else
            {
                UIManager.Instance.ShowPauseMenu();
            }
        }
    }

    public int GetAvailableLevelsCount()
    {
        return levelScenes.Length;
    }

    
    public void InitializeGame(int level = 1)
    {
        Debug.Log($"Initializing game at level {level}");
        currentScore = 0;
        currentLives = startingLives;
        currentLevel = level;
        isGameOver = false;
        Time.timeScale = 1f;

        // Load the corresponding level scene
        if (level > 0 && level <= levelScenes.Length)
        {
            SceneManager.LoadScene(levelScenes[level - 1]);
        }
        else
        {
            Debug.LogError($"Invalid level number: {level}");
            return;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(currentScore);
            UIManager.Instance.UpdateLivesUI(currentLives);
            UIManager.Instance.UpdateCurrentLevelUI(currentLevel);
        }
        
        OnScoreChanged?.Invoke(currentScore);
        OnLivesChanged?.Invoke(currentLives);
        OnLevelChanged?.Invoke(currentLevel);
    }

    public void LoadNextLevel()
    {
        if (currentLevel < levelScenes.Length)
        {
            currentLevel++;
            // Optionally save the current score instead of resetting
            // currentScore = 0;
            currentLives = startingLives;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateCurrentLevelUI(currentLevel);
            }
            
            SceneManager.LoadScene(levelScenes[currentLevel - 1]);
            OnLevelChanged?.Invoke(currentLevel);
        }
        else
        {
            // Player has completed all levels
            GameOver();
        }
    }

    public int GetCurrentScore() => currentScore;
    public int GetCurrentLives() => currentLives;
}