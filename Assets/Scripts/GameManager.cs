using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score { get; private set; }
    public int lives = 3;

    private void Awake()
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

    public void AddScore(int points)
    {
        score += points;
        UIManager.Instance.UpdateScoreUI(score);
    }

    public void LoseLife()
    {
        lives--;
        UIManager.Instance.UpdateLivesUI(lives);

        if (lives <= 0)
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
        // Lógica para manejar el Game Over
        SceneManager.LoadScene("GameOverScene");
    }

    public void ResetBall()
    {
        BallController.Instance.ResetBall();
    }

    public void LoadNextLevel()
    {
        // Implementa la lógica para cargar el siguiente nivel
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextLevelIndex);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
