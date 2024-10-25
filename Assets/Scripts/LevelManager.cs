using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private float victoryDelay = 0.5f; // Pequeño delay antes de mostrar la victoria
    
    private List<GameObject> breakableBlocks;
    private static LevelManager instance;
    private bool isLevelCompleted = false;

    public static LevelManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        SetupInstance();
        InitializeLevel();
    }

    private void SetupInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLevel()
    {
        breakableBlocks = new List<GameObject>();
        isLevelCompleted = false;
    }

    private void Start()
    {
        // Encontrar todos los bloques rompibles al inicio del nivel
        FindAndRegisterBlocks();
    }

    private void FindAndRegisterBlocks()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        breakableBlocks.AddRange(blocks);
        
        Debug.Log($"Found {breakableBlocks.Count} breakable blocks in level");

        // Si no hay bloques, loguear una advertencia
        if (breakableBlocks.Count == 0)
        {
            Debug.LogWarning("No breakable blocks found in level! Make sure blocks are tagged as 'Block'");
        }
    }

    public void RemoveBlock(GameObject block)
    {
        if (!breakableBlocks.Contains(block))
        {
            Debug.LogWarning("Attempted to remove non-registered block");
            return;
        }

        breakableBlocks.Remove(block);
        
        // Verificar si quedan bloques
        if (breakableBlocks.Count == 0 && !isLevelCompleted)
        {
            OnLevelCompleted();
        }
    }

     private void OnLevelCompleted()
    {
        if (isLevelCompleted)
        {
            Debug.Log("Level already completed, ignoring call");
            return;
        }
        
        isLevelCompleted = true;
        Debug.Log("=== Level Completed Called ===");

        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager.Instance is null in OnLevelCompleted!");
            return;
        }

        StartCoroutine(ShowVictoryWithDelay());
    }

     private System.Collections.IEnumerator ShowVictoryWithDelay()
    {
        Debug.Log($"Waiting {victoryDelay} seconds before showing victory screen");
        yield return new WaitForSeconds(victoryDelay);
        
        Debug.Log("Victory delay completed, checking references...");
        
        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager.Instance is null after delay!");
            yield break;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            yield break;
        }

        int currentLevel = GameManager.Instance.GetCurrentLevel();
        Debug.Log($"Showing victory screen for level {currentLevel}");
        UIManager.Instance.ShowVictoryScreen(currentLevel);
    }

    // Métodos públicos para acceder al estado del nivel
    public int GetRemainingBlocks()
    {
        return breakableBlocks.Count;
    }

    public bool IsLevelComplete()
    {
        return isLevelCompleted;
    }

    // Método para reiniciar el estado del nivel si es necesario
    public void ResetLevel()
    {
        breakableBlocks.Clear();
        isLevelCompleted = false;
        FindAndRegisterBlocks();
    }

    private void OnDisable()
    {
        // Limpiar la lista cuando se desactiva el componente
        if (breakableBlocks != null)
        {
            breakableBlocks.Clear();
        }
    }
}
