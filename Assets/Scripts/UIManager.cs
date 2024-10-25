using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [System.Serializable]
    private class UIPanels
    {
        public GameObject mainMenuPanel;
        public GameObject levelSelectPanel;
        public GameObject gameplayPanel;
        public GameObject gameOverPanel;
        public GameObject pausePanel;
        public GameObject victoryPanel ;
    }

    [Header("UI Panels")]
    [SerializeField] private UIPanels panels;
    private UIPanels currentPanels;

    [Header("Level Selection")]
    [SerializeField] private Transform levelButtonsContainer;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private Button backToMainMenuButton;
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

    [Header("Victory UI")]
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button victoryMainMenuButton;
    [SerializeField] private string victoryFormat = "Level {0} Completed!";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private Canvas mainCanvas;
    private bool isInitialized = false;
    private bool isPanelsValid = false;

    private void Awake()
    {
        SetupInstance();
    }

     private void Start()
    {
        if (isInitialized)
        {
            Debug.Log("Start: Setting up buttons");
            SetupButtonListeners(); // Asegurar que se llame aquí también
            SafeHideAllPanels();
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                ShowMainMenu();
                SetupLevelButtons();
            }
            else
            {
                SafeSetPanelActive(currentPanels.gameplayPanel, true);
            }
            Debug.Log("UI Manager initialized");
        }
    }


    private void SetupInstance()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            mainCanvas = GetComponentInParent<Canvas>();
            
            if (mainCanvas != null)
            {
                DontDestroyOnLoad(mainCanvas.gameObject);
            }
            
            InitializePanels();
            ValidateUIReferences();
            
            isInitialized = true;
            SetupButtonListeners();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }

     private void InitializePanels()
    {
        // Crear nueva instancia y copiar referencias
        currentPanels = new UIPanels
        {
            mainMenuPanel = panels.mainMenuPanel,
            levelSelectPanel = panels.levelSelectPanel,
            gameplayPanel = panels.gameplayPanel,
            gameOverPanel = panels.gameOverPanel,
            pausePanel = panels.pausePanel,
            victoryPanel = panels.victoryPanel
        };

        // Validar paneles y registrar información detallada
        isPanelsValid = ValidatePanels();
        LogPanelsStatus();
    }
     private void LogPanelsStatus()
    {
        Debug.Log("=== Estado de Paneles UI ===");
        Debug.Log($"Panel Menú Principal: {(currentPanels.mainMenuPanel != null ? "Válido" : "Faltante")}");
        Debug.Log($"Panel Selección de Nivel: {(currentPanels.levelSelectPanel != null ? "Válido" : "Faltante")}");
        Debug.Log($"Panel de Juego: {(currentPanels.gameplayPanel != null ? "Válido" : "Faltante")}");
        Debug.Log($"Panel Game Over: {(currentPanels.gameOverPanel != null ? "Válido" : "Faltante")}");
        Debug.Log($"Panel de Pausa: {(currentPanels.pausePanel != null ? "Válido" : "Faltante")}");
        Debug.Log($"Panel de Victoria: {(currentPanels.victoryPanel != null ? "Válido" : "Faltante")}");
        Debug.Log($"Paneles Válidos: {isPanelsValid}");
    }




    private void ValidateUIReferences()
    {   
        if (resumeButton == null) Debug.LogError("Resume Button reference is missing!");
        if (mainMenuButton == null) Debug.LogError("Main Menu Button reference is missing!");
        if (scoreText == null) Debug.LogWarning("Score Text reference is missing!");
        if (livesText == null) Debug.LogWarning("Lives Text reference is missing!");
        if (finalScoreText == null) Debug.LogWarning("Final Score Text reference is missing!");
        if (currentLevelText == null) Debug.LogWarning("Current Level Text reference is missing!");
        if (levelButtonPrefab == null) Debug.LogWarning("Level Button Prefab reference is missing!");
        if (levelButtonsContainer == null) Debug.LogWarning("Level Buttons Container reference is missing!");
    }

    private void CachePanelReferences()
    {
        currentPanels = new UIPanels
        {
            mainMenuPanel = panels.mainMenuPanel,
            levelSelectPanel = panels.levelSelectPanel,
            gameplayPanel = panels.gameplayPanel,
            gameOverPanel = panels.gameOverPanel,
            pausePanel = panels.pausePanel
        };
        isPanelsValid = ValidatePanels();
    }

    private bool ValidatePanels()
    {
         if (currentPanels == null)
        {
            Debug.LogError("¡El objeto currentPanels es nulo!");
            return false;
        }

        bool isValid = true;
        
        if (currentPanels.mainMenuPanel == null) 
        {
            Debug.LogError("¡Falta el Panel del Menú Principal!");
            isValid = false;
        }
        if (currentPanels.levelSelectPanel == null)
        {
            Debug.LogError("¡Falta el Panel de Selección de Nivel!");
            isValid = false;
        }
        if (currentPanels.gameplayPanel == null)
        {
            Debug.LogError("¡Falta el Panel de Juego!");
            isValid = false;
        }
        if (currentPanels.gameOverPanel == null)
        {
            Debug.LogError("¡Falta el Panel de Game Over!");
            isValid = false;
        }
        if (currentPanels.pausePanel == null)
        {
            Debug.LogError("¡Falta el Panel de Pausa!");
            isValid = false;
        }
        if (currentPanels.victoryPanel == null)
        {
            Debug.LogError("¡Falta el Panel de Victoria!");
            isValid = false;
        }

        return isValid;
               
    }

    private void OnEnable()
    {
        if (isInitialized && !isPanelsValid)
        {
            CachePanelReferences();
        }
    }

    public void ShowVictoryScreen(int level)
{
     Debug.Log("=== Show Victory Screen Called ===");
    Debug.Log($"Current Level: {level}");
    
    if (currentPanels == null)
    {
        Debug.LogError("currentPanels is null!");
        return;
    }

    if (currentPanels.victoryPanel == null)
    {
        Debug.LogError("victoryPanel is null!");
        return;
    }

    SafeHideAllPanels();
    Debug.Log("All panels hidden, attempting to show victory panel");
    
    SafeSetPanelActive(currentPanels.victoryPanel, true);
    Debug.Log($"Victory Panel Active State: {currentPanels.victoryPanel.activeSelf}");
    
    if (victoryText != null)
    {
        victoryText.text = string.Format(victoryFormat, level);
        Debug.Log($"Victory Text Updated: {victoryText.text}");
    }
    else
    {
        Debug.LogError("victoryText is null!");
    }
    
    // Verificar si hay más niveles disponibles
    if (GameManager.Instance != null)
    {
        bool hasNextLevel = level < GameManager.Instance.GetAvailableLevelsCount();
        Debug.Log($"Has Next Level: {hasNextLevel}");
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(hasNextLevel);
        }
    }
    else
    {
        Debug.LogError("GameManager Instance is null!");
    }
}   

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("¡UIManager no inicializado durante la carga de la escena!");
            return;
        }

        // Intentar recuperar referencias de paneles
        if (!isPanelsValid)
        {
            Debug.Log("Intentando recuperar referencias de Paneles UI...");
            InitializePanels();
            
            if (!isPanelsValid)
            {
                Debug.LogError("¡Error al recuperar referencias de Paneles UI! Las referencias pueden haberse perdido durante la transición de escena.");
                LogPanelsStatus();
                return;
            }
        }

        // Manejar configuración específica de la escena
        if (scene.name == "MainMenu")
        {
            SetupLevelButtons();
            ShowMainMenu();
        }
        else
        {
            SafeHideAllPanels();
            SafeSetPanelActive(currentPanels.gameplayPanel, true);
            UpdateUIElements();
        }
    }
    

   

    

    private void UpdateUIElements()
    {
        if (GameManager.Instance != null)
        {
            UpdateScoreUI(GameManager.Instance.GetCurrentScore());
            UpdateLivesUI(GameManager.Instance.GetCurrentLives());
        }
    }

    private void SetupButtonListeners()
    {
        Debug.Log("Setting up button listeners - BEGIN");


        SafeAddListener(startGameButton, StartGame);
        SafeAddListener(restartButton, RestartGame);
        SafeAddListener(mainMenuButton, ReturnToMainMenu);
        SafeAddListener(resumeButton, ResumeGame);
        
        SafeAddListener(quitButton, QuitGame);
        SafeAddListener(openLevelSelectButton, ShowLevelSelect);
        SafeAddListener(backToMainMenuButton, ReturnToMainMenu);
         if (nextLevelButton != null)
     {
        nextLevelButton.onClick.RemoveAllListeners();
        nextLevelButton.onClick.AddListener(() => {
            PlayButtonSound();
            GameManager.Instance?.LoadNextLevel();
        });
     }
    
    if (victoryMainMenuButton != null)
    {
        victoryMainMenuButton.onClick.RemoveAllListeners();
        victoryMainMenuButton.onClick.AddListener(() => {
            PlayButtonSound();
            ReturnToMainMenu();
        });
    }

        Debug.Log("Setting up button listeners - END");

    }
    
    private void OnValidate()
    {
        Debug.Log("Validating button references");
        if (restartButton == null)
            Debug.LogError("Restart Button is not assigned in the inspector!");
        if (mainMenuButton == null)
            Debug.LogError("Main Menu Button is not assigned in the inspector!");
        if (resumeButton == null)
            Debug.LogError("Resume Button is not assigned in the inspector!");
        if (startGameButton == null)
            Debug.LogError("Start Game Button is not assigned in the inspector!");
           if (quitButton == null)
            Debug.LogError("Quit Button is not assigned in the inspector!");
        if (openLevelSelectButton == null)
            Debug.LogError("Open Level Select Button is not assigned in the inspector!");
        if (backToMainMenuButton == null)
            Debug.LogError("Back to Main Menu Button is not assigned in the inspector!");    
    }


    public void RestartGame()
    {
        Debug.Log("Attempting to restart game");
        if (GameManager.Instance == null)
        {
            Debug.LogError("Cannot restart game - GameManager.Instance is null");
            return;
        }
        
        SafeHideAllPanels();
        SafeSetPanelActive(currentPanels.gameplayPanel, true);
        Time.timeScale = 1f;  // Asegurarnos que el tiempo está corriendo
        GameManager.Instance.RestartGame();
    }

public void ReturnToMainMenu()
{
    Debug.Log("Attempting to return to main menu");
        if (GameManager.Instance == null)
        {
            Debug.LogError("Cannot return to main menu - GameManager.Instance is null");
            SceneManager.LoadScene("MainMenu");
            return;
        }
        
        SafeHideAllPanels();
        Time.timeScale = 1f;  // Asegurarnos que el tiempo está corriendo
        GameManager.Instance.LoadMainMenu();
} 

public void ResumeGame()
    {
        Debug.Log("Resuming game");
        Time.timeScale = 1f;
        SafeSetPanelActive(currentPanels.pausePanel, false);
        Debug.Log("Game resumed - Time scale set to 1");
    }



  private void SafeAddListener(Button button, UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                PlayButtonSound();
                action.Invoke();
            });
        }
    }

    private void SetupLevelButtons()
    {
        if (levelButtonsContainer == null || levelButtonPrefab == null) return;

        foreach (Transform child in levelButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        if (GameManager.Instance == null) return;

        int availableLevels = GameManager.Instance.GetAvailableLevelsCount();
        for (int i = 1; i <= availableLevels; i++)
        {
            Button levelButton = Instantiate(levelButtonPrefab, levelButtonsContainer);
            int levelIndex = i;
            
            TextMeshProUGUI buttonText = levelButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"Level {i}";
            }

            levelButton.onClick.AddListener(() => {
                PlayButtonSound();
                StartGameAtLevel(levelIndex);
            });
        }
    }

    public void ShowLevelSelect()
    {
        Debug.Log("Showing level select menu");
        PlayButtonSound();
        SafeHideAllPanels();
        SafeSetPanelActive(currentPanels.levelSelectPanel, true);
        SetupLevelButtons(); 
    }

    private void StartGameAtLevel(int level)
    {
        Debug.Log($"Starting game at level {level}");
        SafeHideAllPanels();
        SafeSetPanelActive(currentPanels.gameplayPanel, true);
        GameManager.Instance?.InitializeGame(level);
    }

    public void ShowMainMenu()
    {
       Debug.Log("Showing main menu");
        SafeHideAllPanels();
        SafeSetPanelActive(currentPanels.mainMenuPanel, true);
        
        
    }

    public void StartGame()
    {
        Debug.Log("Starting game");
        SafeHideAllPanels();
        SafeSetPanelActive(currentPanels.gameplayPanel, true);
        GameManager.Instance?.InitializeGame(1);
    }

    

    public void ShowGameOver(int finalScore)
    {
        Debug.Log("Showing game over");
        SafeHideAllPanels();
        SafeSetPanelActive(currentPanels.gameOverPanel, true);
        
        if (finalScoreText != null)
        {
            finalScoreText.text = string.Format(finalScoreFormat, finalScore);
        }
    }

    public void ShowPauseMenu()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" && 
            currentPanels?.pausePanel != null)
        {
            SafeSetPanelActive(currentPanels.pausePanel, true);
            Debug.Log("Pause menu shown");
        }
    }

    public void HidePauseMenu()
    {
        if (currentPanels?.pausePanel != null)
        {
            SafeSetPanelActive(currentPanels.pausePanel, false);
            Debug.Log("Pause menu hidden");
        }
    }

    private void SafeHideAllPanels()
    {
        if (!isPanelsValid) return;
        
        SafeSetPanelActive(currentPanels.mainMenuPanel, false);
        SafeSetPanelActive(currentPanels.levelSelectPanel, false);
        SafeSetPanelActive(currentPanels.gameplayPanel, false);
        SafeSetPanelActive(currentPanels.gameOverPanel, false);
        SafeSetPanelActive(currentPanels.pausePanel, false);
        SafeSetPanelActive(currentPanels.victoryPanel, false);
        
        Debug.Log("All panels hidden");
    }

    private void SafeSetPanelActive(GameObject panel, bool active)
    {
        if (panel != null && panel)
        {
            try
            {
                panel.SetActive(active);
            }
            catch (MissingReferenceException e)
            {
                Debug.LogWarning($"Failed to set panel active state: {e.Message}");
                isPanelsValid = false;
            }
        }
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.SetText(string.Format(scoreFormat, score));
        }
    }

    public void UpdateLivesUI(int lives)
    {
        if (livesText != null)
        {
            livesText.SetText(string.Format(livesFormat, lives));
        }
    }

    public void UpdateCurrentLevelUI(int level)
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = $"Level: {level}";
        }
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
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

    public void DebugCheckVictoryPanel()
    {
    Debug.Log("=== Victory Panel Debug Info ===");
    Debug.Log($"Victory Panel Reference: {(currentPanels.victoryPanel != null ? "OK" : "MISSING")}");
    Debug.Log($"Victory Text Reference: {(victoryText != null ? "OK" : "MISSING")}");
    Debug.Log($"Next Level Button: {(nextLevelButton != null ? "OK" : "MISSING")}");
    Debug.Log($"Victory Main Menu Button: {(victoryMainMenuButton != null ? "OK" : "MISSING")}");
    Debug.Log($"Current Panels Valid: {isPanelsValid}");
    
    }



    private void OnDestroy()
    {
        if (isInitialized)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
