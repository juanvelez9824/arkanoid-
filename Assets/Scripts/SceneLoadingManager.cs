using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class SceneLoadingManager : MonoBehaviour
{
    public static SceneLoadingManager Instance { get; private set; }

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private float minimumLoadingTime = 0.5f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private CanvasGroup loadingCanvasGroup;

    private bool isLoading = false;

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

    public void LoadSceneAsync(string sceneName, System.Action onLoadComplete = null)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, onLoadComplete));
        }
    }

    private IEnumerator LoadSceneRoutine(string sceneName, System.Action onLoadComplete)
    {
        isLoading = true;

        // Activar y hacer fade in de la pantalla de carga
        loadingScreen.SetActive(true);
        loadingCanvasGroup.alpha = 0f;

        // Fade in
        while (loadingCanvasGroup.alpha < 1f)
        {
            loadingCanvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        float elapsedTime = 0f;
        float startTime = Time.realtimeSinceStartup;

        // Comenzar a cargar la escena en segundo plano
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            elapsedTime = Time.realtimeSinceStartup - startTime;
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            if (progressBar != null)
                progressBar.fillAmount = progress;

            if (progressText != null)
                progressText.text = $"Cargando... {(progress * 100):0}%";

            // Esperar el tiempo mínimo y que la carga esté lista
            if (asyncOperation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Fade out
        while (loadingCanvasGroup.alpha > 0f)
        {
            loadingCanvasGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        loadingScreen.SetActive(false);
        isLoading = false;
        onLoadComplete?.Invoke();
    }
}
