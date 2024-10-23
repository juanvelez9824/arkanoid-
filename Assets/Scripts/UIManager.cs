using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Level Selection")]
    [SerializeField] private Transform levelButtonsContainer; // Parent object for level buttons
    [SerializeField] private Button levelButtonPrefab; // Prefab for level buttons
    [SerializeField] private Button backToMainMenuButton; // Button to return from level selection
    [SerializeField] private Button openLevelSelectButton;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI currentLevelText;

    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    [Header("UI Format")]
    [SerializeField] private string scoreFormat = "Score: {0}";
    [SerializeField] private string livesFormat = "Lives: {0}";
    [SerializeField] private string finalScoreFormat = "Final Score: {0}";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;


    private void Awake()
    {
        SetupInstance();
        ValidateReferences();
        SetupButtonListeners();
        SetupLevelButtons();
    }

    private void Start()
    {
        // Inicializar UI
        HideAllPanels();
        ShowMainMenu();
        Debug.Log("UI Manager initialized");
    }

    private void SetupInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupLevelButtons()
    {
        // Get available levels from GameManager
        int availableLevels = GameManager.Instance.GetAvailableLevelsCount();

        // Create button for each level
        for (int i = 1; i <= availableLevels; i++)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonsContainer);
            int levelIndex = i; // Store level index for the button click
            
            // Set button text
            TextMeshProUGUI buttonText = levelButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"Level {i}";
            }

            // Add click listener
            levelButton.onClick.AddListener(() => {
                PlayButtonSound();
                StartGameAtLevel(levelIndex);
            });
        }
    }





    private void ValidateReferences()
    {
        // Validar panels
        Debug.Assert(mainMenuPanel != null, "Main Menu Panel is not assigned!");
        Debug.Assert(gameplayPanel != null, "Gameplay Panel is not assigned!");
        Debug.Assert(gameOverPanel != null, "Game Over Panel is not assigned!");
        Debug.Assert(pausePanel != null, "Pause Panel is not assigned!");

        // Validar UI elements
        Debug.Assert(scoreText != null, "Score Text is not assigned!");
        Debug.Assert(livesText != null, "Lives Text is not assigned!");
        Debug.Assert(finalScoreText != null, "Final Score Text is not assigned!");

        // Validar buttons
        Debug.Assert(startGameButton != null, "Start Game Button is not assigned!");
        Debug.Assert(restartButton != null, "Restart Button is not assigned!");
        Debug.Assert(mainMenuButton != null, "Main Menu Button is not assigned!");
        Debug.Assert(resumeButton != null, "Resume Button is not assigned!");
        Debug.Assert(quitButton != null, "Quit Button is not assigned!");
    }

    private void SetupButtonListeners()
    {
        // Remover listeners existentes y añadir nuevos
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(() => {
                PlayButtonSound();
                StartGame();
            });
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => {
                PlayButtonSound();
                RestartGame();
            });
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(() => {
                PlayButtonSound();
                ReturnToMainMenu();
            });
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(() => {
                PlayButtonSound();
                ResumeGame();
            });
        }

        if (openLevelSelectButton != null)
        {
            openLevelSelectButton.onClick.RemoveAllListeners();
            openLevelSelectButton.onClick.AddListener(() => {
                PlayButtonSound();
                ShowLevelSelect();
            });
        }
        if (backToMainMenuButton != null)
        {
            backToMainMenuButton.onClick.RemoveAllListeners();
            backToMainMenuButton.onClick.AddListener(() => {
                PlayButtonSound();
                ShowMainMenu();
            });
        }


        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => {
                PlayButtonSound();
                QuitGame();
            });
        }
    }
    public void ShowLevelSelect()
    {
        HideAllPanels();
        levelSelectPanel?.SetActive(true);
        Debug.Log("Showing level select menu");
    }

    private void StartGameAtLevel(int level)
    {
        Debug.Log($"Starting game at level {level}");
        HideAllPanels();
        gameplayPanel?.SetActive(true);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeGame(level);
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private void HideAllPanels()
    {
        mainMenuPanel?.SetActive(false);
        levelSelectPanel?.SetActive(false);
        gameplayPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        Debug.Log("All panels hidden");
    }


    public void ShowMainMenu()
    {
        Debug.Log("Showing main menu");
        HideAllPanels();
        mainMenuPanel?.SetActive(true);
    }

    public void StartGame()
    {
        Debug.Log("Starting game");
        HideAllPanels();
        gameplayPanel?.SetActive(true);
         
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeGame();
            
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }

    private void RestartGame()
    {
        Debug.Log("Restarting game");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    private void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
    }

    private void ResumeGame()
    {
        Debug.Log("Resuming game");
        Time.timeScale = 1f;
        pausePanel?.SetActive(false);
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = string.Format(scoreFormat, score);
        }
    }

    public void UpdateLivesUI(int lives)
    {
        if (livesText != null)
        {
            livesText.text = string.Format(livesFormat, lives);
            AnimateLivesText();
        }
    }

    public void ShowGameOver(int finalScore)
    {
        Debug.Log("Showing game over");
        HideAllPanels();
        gameOverPanel?.SetActive(true);
        
        if (finalScoreText != null)
        {
            finalScoreText.text = string.Format(finalScoreFormat, finalScore);
        }
    }

    public void ShowPauseMenu()
    {
        pausePanel?.SetActive(true);
        Debug.Log("Pause menu shown");
    }

    public void HidePauseMenu()
    {
        pausePanel?.SetActive(false);
        Debug.Log("Pause menu hidden");
    }

    private void AnimateLivesText()
    {
        if (livesText != null)
        {
            Color originalColor = livesText.color;
            StartCoroutine(PulseTextColor(livesText, originalColor, Color.red, 0.2f));
        }
    }

    private IEnumerator PulseTextColor(TextMeshProUGUI text, Color originalColor, Color targetColor, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.color = Color.Lerp(originalColor, targetColor, elapsed / duration);
            yield return null;
        }

        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.color = Color.Lerp(targetColor, originalColor, elapsed / duration);
            yield return null;
        }

        text.color = originalColor;
    }

    public bool IsAnyPanelActive()
    {

        return mainMenuPanel.activeSelf || gameplayPanel.activeSelf || gameOverPanel.activeSelf || pausePanel.activeSelf;
    }

    public void UpdateCurrentLevelUI(int level)
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = $"Level: {level}";
        }
    }










}