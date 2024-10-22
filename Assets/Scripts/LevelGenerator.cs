using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] blockPrefabs; // Prefabs de diferentes tipos de bloques
    public int minRows = 3; // Número mínimo de filas de bloques
    public int maxRows = 6; // Número máximo de filas de bloques
    public int minColumns = 5; // Número mínimo de columnas de bloques
    public int maxColumns = 12; // Número máximo de columnas de bloques
    public float blockWidth = 1.5f; // Ancho de los bloques
    public float blockHeight = 0.5f; // Altura de los bloques
    public Vector2 startPos = new Vector2(-7, 4); // Posición inicial de la primera fila de bloques
    public float emptySpaceProbability = 0.1f; // Probabilidad de dejar un espacio vacío

    private int rows;
    private int columns;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Establece un número de filas y columnas aleatorio para cada nivel dentro del rango definido
        rows = Random.Range(minRows, maxRows + 1);
        columns = Random.Range(minColumns, maxColumns + 1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Decidir aleatoriamente si dejar un espacio vacío
                if (Random.Range(0f, 1f) < emptySpaceProbability)
                {
                    continue; // Deja este espacio vacío
                }

                // Genera patrones específicos, por ejemplo, líneas, zigzag o bloques en forma de 'X'
                if (ShouldSkipBlockInPattern(row, col))
                {
                    continue; // Salta este bloque para formar un patrón específico
                }

                // Decide aleatoriamente qué tipo de bloque generar
                int randomIndex = Random.Range(0, blockPrefabs.Length);
                GameObject blockPrefab = blockPrefabs[randomIndex];

                // Calcula la posición de cada bloque
                Vector3 spawnPosition = new Vector3(startPos.x + col * blockWidth, startPos.y - row * blockHeight, 0);

                // Instancia el bloque en la posición calculada
                Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    // Función para aplicar patrones específicos de generación de bloques
    bool ShouldSkipBlockInPattern(int row, int col)
    {
        // Ejemplo de un patrón en zigzag: saltar bloques en columnas impares en filas pares
        if (row % 2 == 0 && col % 2 != 0)
        {
            return true;
        }

        // Ejemplo de un patrón en forma de 'X': dejar huecos en las esquinas
        if (row == col || row + col == columns - 1)
        {
            return true;
        }

        // Puedes añadir más patrones aquí
        return false;
    }
}