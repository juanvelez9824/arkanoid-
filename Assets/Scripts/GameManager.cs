using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public int startingLives = 3;
    public int currentLevel = 1;
    public int maxLevels = 2;
    
    [Header("UI References")]
    public Text scoreText;
    public Text livesText;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    
    private int currentScore;
    private int currentLives;
    private int remainingBlocks;
    
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
    
    void Start()
    {
        InitializeGame();
    }
    
    public void InitializeGame()
    {
        currentScore = 0;
        currentLives = startingLives;
        UpdateUI();
        CountBlocks();
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }
    
    public void LoseLife()
    {
        currentLives--;
        UpdateUI();
        
        if (currentLives <= 0)
        {
            GameOver();
        }
    }
    
    public void BlockDestroyed()
    {
        remainingBlocks--;
        if (remainingBlocks <= 0)
        {
            LevelComplete();
        }
    }
    
    private void CountBlocks()
    {
        remainingBlocks = FindObjectsOfType<Block>().Length;
    }
    
    private void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {currentScore}";
        if (livesText) livesText.text = $"Lives: {currentLives}";
    }
    
    private void GameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        AudioManager.Instance?.PlaySound("gameOver");
    }
    
    private void LevelComplete()
    {
        if (currentLevel >= maxLevels)
        {
            if (victoryPanel) victoryPanel.SetActive(true);
        }
        else
        {
            currentLevel++;
            SceneManager.LoadScene($"Level{currentLevel}");
        }
    }
}