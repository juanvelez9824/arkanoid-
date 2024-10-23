using UnityEngine;

public class TestManager : MonoBehaviour
{
    // Referencia al GameManager para pruebas
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado!");
        }
    }

    // Métodos para probar desde el inspector o con teclas
    public void TestAddScore()
    {
        if (gameManager != null)
        {
            gameManager.AddScore(100);
            Debug.Log("Añadiendo 100 puntos");
        }
    }

    public void TestLoseLife()
    {
        if (gameManager != null)
        {
            gameManager.LoseLife();
            Debug.Log("Perdiendo una vida");
        }
    }

    // Update para pruebas con teclado
    private void Update()
    {
        // Presiona S para añadir score
        if (Input.GetKeyDown(KeyCode.S))
        {
            TestAddScore();
        }

        // Presiona L para perder una vida
        if (Input.GetKeyDown(KeyCode.L))
        {
            TestLoseLife();
        }

        // Presiona R para reiniciar
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager?.RestartGame();
        }
    }
}
