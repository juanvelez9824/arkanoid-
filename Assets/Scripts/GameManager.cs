using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int startingLives = 3;
    
    private int currentScore;
    private int currentLives;
    private bool isGameOver;

    private void Awake()
    {
        SetupInstance();
    }

    private void Start()
    {
        InitializeGame();
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

    private void InitializeGame()
    {
        currentScore = 0;
        currentLives = startingLives;
        isGameOver = false;

        // Actualizar UI inicial
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(currentScore);
            UIManager.Instance.UpdateLivesUI(currentLives);
            UIManager.Instance.HideGameOver();
        }
    }
    public void AddLife()
{
    if (isGameOver) return;
    
    currentLives++;
    UIManager.Instance?.UpdateLivesUI(currentLives);
}

    public void AddScore(int points)
    {
        if (isGameOver) return;

        currentScore += points;
        UIManager.Instance?.UpdateScoreUI(currentScore);
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        UIManager.Instance?.UpdateLivesUI(currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            ResetBall();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        UIManager.Instance?.ShowGameOver(currentScore);
    }

    public void ResetBall()
    {
        if (BallController.Instance != null)
        {
            BallController.Instance.ResetBall();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }
}