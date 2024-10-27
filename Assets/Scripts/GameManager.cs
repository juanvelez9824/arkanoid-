using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private KeyCode pauseKey = KeyCode.P;
    [SerializeField] private string[] levelScenes;

    [Header("Power-Up Settings")]
    [SerializeField] private float powerUpDuration = 10f;
    
    public event Action<PowerUpType> OnPowerUpCollected;
    public event Action<PowerUpType> OnPowerUpExpired;

    private bool isPaddlePowerUpActive;
    private float paddlePowerUpTimeRemaining;

    
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action OnGameOver;
    
    public event Action OnGameRestart;
     public event Action<int> OnLevelChanged;
     public event Action OnVictory;


    private int currentLevel;
    private int currentScore;
    private int currentLives;
    private bool isGameOver;
    private bool isPaused;

    private void Awake()
    {
        SetupInstance();
        InitializeGameState();
    }

     private void InitializeGameState()
    {
        currentScore = 0;
        currentLives = startingLives;
        currentLevel = 1;
        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        UpdateUIState();
        
        // Si estamos en el menú principal, asegurarnos que el tiempo corre
        if (scene.name == "MainMenu")
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
       
    }
    private void UpdateUIState()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(currentScore);
            UIManager.Instance.UpdateLivesUI(currentLives);
            UIManager.Instance.UpdateCurrentLevelUI(currentLevel);
            
            // Asegurarse de que el menú de pausa esté en el estado correcto
            if (isPaused)
            {
                UIManager.Instance.ShowPauseMenu();
            }
            else
            {
                UIManager.Instance.HidePauseMenu();
            }
        }
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
            Debug.Log("GameManager instance created");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameManager destroyed");
        }
    }

    public void AddLife()
    {
        if (isGameOver) return;

        currentLives++;
        UIManager.Instance?.UpdateLivesUI(currentLives);
        OnLivesChanged?.Invoke(currentLives);
    }

    public void ActivatePowerUp(PowerUpType type)
    {
        OnPowerUpCollected?.Invoke(type);
        
        switch (type)
        {
            case PowerUpType.PaddleSize:
                StartCoroutine(HandlePaddlePowerUpDuration());
                break;
            case PowerUpType.BallSpeed:
                // La lógica está en el PowerUp script
                break;
            case PowerUpType.ExtraLife:
                AddLife();
                break;
            case PowerUpType.ExplodingBlocks:
                // La lógica está en el PowerUp script
                break;
        }
    }
    private IEnumerator HandlePaddlePowerUpDuration()
    {
        isPaddlePowerUpActive = true;
        paddlePowerUpTimeRemaining = powerUpDuration;

        while (paddlePowerUpTimeRemaining > 0)
        {
            if (!isPaused)
            {
                paddlePowerUpTimeRemaining -= Time.deltaTime;
            }
            yield return null;
        }

        isPaddlePowerUpActive = false;
        OnPowerUpExpired?.Invoke(PowerUpType.PaddleSize);
    }

    // Método para verificar si un power-up está activo
    public bool IsPowerUpActive(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.PaddleSize:
                return isPaddlePowerUpActive;
            default:
                return false;
        }
    }

    // Método para obtener el tiempo restante de un power-up
    public float GetPowerUpTimeRemaining(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.PaddleSize:
                return isPaddlePowerUpActive ? paddlePowerUpTimeRemaining : 0f;
            default:
                return 0f;
        }
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame called");
        
        // Reiniciar variables
        currentScore = 0;
        currentLives = startingLives;
        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;
        isPaddlePowerUpActive = false;
        paddlePowerUpTimeRemaining = 0f;
        StopAllCoroutines(); // Detener cualquier power-up activo
        
        // Notificar a los observadores
        OnGameRestart?.Invoke();
        OnScoreChanged?.Invoke(currentScore);
        OnLivesChanged?.Invoke(currentLives);
        
        // Cargar el nivel actual o el primero si no hay nivel válido
        if (currentLevel > 0 && currentLevel <= levelScenes.Length)
        {
            Debug.Log($"Loading level scene: {levelScenes[currentLevel - 1]}");
            SceneManager.LoadScene(levelScenes[currentLevel - 1]);
        }
        else
        {
            Debug.Log("Resetting to first level");
            currentLevel = 1;
            if (levelScenes.Length > 0)
            {
                SceneManager.LoadScene(levelScenes[0]);
            }
            else
            {
                Debug.LogError("No level scenes configured in GameManager!");
            }
        }
        
        OnLevelChanged?.Invoke(currentLevel);
        
        // Actualizar UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(currentScore);
            UIManager.Instance.UpdateLivesUI(currentLives);
            UIManager.Instance.UpdateCurrentLevelUI(currentLevel);
        }
        else
        {
            Debug.LogWarning("UIManager instance not found during restart");
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
        if (!isGameOver) // Evita que se llame múltiples veces
        {
            isGameOver = true;
            OnGameOver?.Invoke();
            
            // Solo reproducir el sonido de game over si no es una victoria
            if (currentLevel < levelScenes.Length)
            {
                AudioManager.Instance?.PlayGameOverSound();
            }
            
            UIManager.Instance?.ShowGameOver(currentScore);
            Debug.Log($"Game Over! Final Score: {currentScore}");
        }

    }

    

    public void LoadMainMenu()
    {
        Debug.Log("LoadMainMenu called");
        
        // Resetear el estado del juego
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        currentScore = 0;
        currentLives = startingLives; 

        SceneLoadingManager.Instance?.LoadSceneAsync("MainMenu");
        
        try 
        {
            SceneManager.LoadScene("MainMenu");
            Debug.Log("MainMenu scene loaded successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load MainMenu scene: {e.Message}");
            Debug.LogError("Make sure 'MainMenu' scene is in Build Settings!");
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (UIManager.Instance != null)
        {
            try
            {
                if (isPaused)
                {
                    UIManager.Instance.ShowPauseMenu();
                }
                else
                {
                    UIManager.Instance.HidePauseMenu();
                }
            }
            catch (System.NullReferenceException e)
            {
                Debug.LogWarning($"UI Reference lost during pause toggle: {e.Message}");
                // Revertir el estado de pausa si falló
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0f : 1f;
            }
        }
        else
        {
            Debug.LogWarning("UIManager.Instance is null during pause toggle");
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
           SceneLoadingManager.Instance?.LoadSceneAsync(levelScenes[level - 1]);
        }
        else
        {
            Debug.LogError($"Invalid level number: {level}");
            return;
        }
        
        OnScoreChanged?.Invoke(currentScore);
        OnLivesChanged?.Invoke(currentLives);
        OnLevelChanged?.Invoke(currentLevel);
    }

    
    private void Victory()
    {
        Debug.Log("Victory achieved!");
        isGameOver = true;
        OnVictory?.Invoke();
        AudioManager.Instance?.PlayVictorySound();
        
        // Esperar antes de mostrar la pantalla de game over
        StartCoroutine(DelayedGameOverAfterVictory());
    }

    private IEnumerator DelayedGameOverAfterVictory()
    {
        yield return new WaitForSeconds(3f); // Ajusta este tiempo según la duración de tu sonido de victoria
        UIManager.Instance?.ShowGameOver(currentScore);
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
             AudioManager.Instance?.PlayVictorySound();
        }
        else
        {   
           
            
           Victory();
        }
    }

    

    public int GetCurrentScore() => currentScore;
    public int GetCurrentLives() => currentLives;
    public bool IsGamePaused() => isPaused;
    
}