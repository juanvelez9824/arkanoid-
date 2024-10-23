using UnityEngine;
using TMPro; // Asegúrate de tener el paquete TextMeshPro importado
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("UI Format Settings")]
    [SerializeField] private string scoreFormat = "Score: {0}";
    [SerializeField] private string livesFormat = "Lives: {0}";
    [SerializeField] private string finalScoreFormat = "Final Score: {0}";

    private void Awake()
    {
        SetupInstance();
        ValidateReferences();
    }

    private void Start()
    {
        // Inicializar UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Actualizar UI inicial
        UpdateScoreUI(0);
        UpdateLivesUI(3);
    }

    private void SetupInstance()
    {
        if (Instance == null)
        {
            Instance = this;
            // No usar DontDestroyOnLoad aquí ya que queremos un UIManager por escena
        }
        else
        {
            Debug.LogWarning($"Multiple UIManager instances detected. Destroying duplicate on {gameObject.name}");
            Destroy(gameObject);
        }
    }

    private void ValidateReferences()
    {
        bool hasErrors = false;

        if (scoreText == null)
        {
            Debug.LogError("ScoreText reference is missing in UIManager");
            hasErrors = true;
        }

        if (livesText == null)
        {
            Debug.LogError("LivesText reference is missing in UIManager");
            hasErrors = true;
        }

        if (hasErrors)
        {
            Debug.LogError("Please assign all required UI references in the inspector for UIManager");
        }
    }

    public void UpdateScoreUI(int score)
    {
        try
        {
            if (scoreText != null)
            {
                scoreText.text = string.Format(scoreFormat, score);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating score UI: {e.Message}");
        }
    }

    public void UpdateLivesUI(int lives)
    {
        try
        {
            if (livesText != null)
            {
                livesText.text = string.Format(livesFormat, lives);
                
                // Efecto visual cuando pierdes vidas
                if (lives > 0)
                {
                    AnimateLivesText();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating lives UI: {e.Message}");
        }
    }

    private void AnimateLivesText()
    {
        if (livesText != null)
        {
            // Guardar el color original
            Color originalColor = livesText.color;
            
            // Aplicar animación usando DOTween si está disponible
            #if DOTWEEN_ENABLED
            livesText.DOColor(Color.red, 0.2f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => livesText.color = originalColor);
            #else
            // Animación básica sin DOTween
            StartCoroutine(PulseTextColor(livesText, originalColor, Color.red, 0.2f));
            #endif
        }
    }

    private System.Collections.IEnumerator PulseTextColor(TextMeshProUGUI text, Color originalColor, Color targetColor, float duration)
    {
        // Animación de ida
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.color = Color.Lerp(originalColor, targetColor, elapsed / duration);
            yield return null;
        }

        // Animación de vuelta
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.color = Color.Lerp(targetColor, originalColor, elapsed / duration);
            yield return null;
        }

        text.color = originalColor;
    }

    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null)
            {
                finalScoreText.text = string.Format(finalScoreFormat, finalScore);
            }
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Método para animar el score cuando aumenta
    private void AnimateScoreIncrease()
    {
        if (scoreText != null)
        {
            // Guardar la escala original
            Vector3 originalScale = scoreText.transform.localScale;
            
            // Animación básica de escala
            StartCoroutine(PulseTextScale(scoreText.transform, originalScale, originalScale * 1.2f, 0.2f));
        }
    }

    private System.Collections.IEnumerator PulseTextScale(Transform textTransform, Vector3 originalScale, Vector3 targetScale, float duration)
    {
        // Animación de incremento de escala
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        // Animación de vuelta a escala original
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }

        textTransform.localScale = originalScale;
    }
}