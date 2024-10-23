using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public class BlockType
    {
        public GameObject prefab;
        public float probability = 1f; // Probabilidad de aparición de este tipo de bloque
        public int points = 100; // Puntos por destruir este bloque
        public bool isIndestructible = false;
    }

    [System.Serializable]
    public enum LevelPattern
    {
        Random,
        Checkerboard,
        ZigZag,
        Diamond,
        Walls,
        Pyramid,
        Spiral
    }

    [Header("Nivel")]
    public BlockType[] blockTypes;
    public LevelPattern pattern = LevelPattern.Random;
    public bool randomizePattern = true; // Si queremos que el patrón sea aleatorio
    
    [Header("Dimensiones")]
    public int minRows = 3;
    public int maxRows = 6;
    public int minColumns = 5;
    public int maxColumns = 12;
    public float blockWidth = 1.5f;
    public float blockHeight = 0.5f;
    public float spacing = 0.1f; // Espacio entre bloques
    
    [Header("Posicionamiento")]
    public Vector2 startPos = new Vector2(-7, 4);
    public float emptySpaceProbability = 0.1f;
    
    [Header("Dificultad")]
    public bool increaseDifficultyWithLevel = true;
    public float difficultyMultiplier = 1.1f;
    public int currentLevel = 1;
    
    private int rows;
    private int columns;
    private List<GameObject> currentBlocks = new List<GameObject>();

    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        // Limpiar nivel anterior
        ClearLevel();
        
        // Ajustar dificultad según el nivel
        AdjustDifficultyForLevel();
        
        // Establecer dimensiones del nivel
        rows = Random.Range(minRows, maxRows + 1);
        columns = Random.Range(minColumns, maxColumns + 1);

        // Seleccionar patrón
        if (randomizePattern)
        {
            pattern = (LevelPattern)Random.Range(0, System.Enum.GetValues(typeof(LevelPattern)).Length);
        }

        // Generar matriz de nivel
        bool[,] levelMatrix = GenerateLevelMatrix();
        
        // Crear bloques según la matriz
        SpawnBlocks(levelMatrix);
    }

    private void ClearLevel()
    {
        foreach (GameObject block in currentBlocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }
        currentBlocks.Clear();
    }

    private void AdjustDifficultyForLevel()
    {
        if (increaseDifficultyWithLevel)
        {
            // Reducir probabilidad de espacios vacíos con cada nivel
            emptySpaceProbability = Mathf.Max(0.05f, emptySpaceProbability - (0.01f * currentLevel));
            
            // Aumentar probabilidad de bloques más resistentes
            foreach (BlockType block in blockTypes)
            {
                if (block.isIndestructible)
                {
                    block.probability = Mathf.Min(0.3f, block.probability + (0.02f * currentLevel));
                }
            }
        }
    }

    private bool[,] GenerateLevelMatrix()
    {
        bool[,] matrix = new bool[rows, columns];
        
        switch (pattern)
        {
            case LevelPattern.Random:
                GenerateRandomPattern(matrix);
                break;
            case LevelPattern.Checkerboard:
                GenerateCheckerboardPattern(matrix);
                break;
            case LevelPattern.ZigZag:
                GenerateZigZagPattern(matrix);
                break;
            case LevelPattern.Diamond:
                GenerateDiamondPattern(matrix);
                break;
            case LevelPattern.Walls:
                GenerateWallsPattern(matrix);
                break;
            case LevelPattern.Pyramid:
                GeneratePyramidPattern(matrix);
                break;
            case LevelPattern.Spiral:
                GenerateSpiralPattern(matrix);
                break;
        }
        
        return matrix;
    }

    private void GenerateRandomPattern(bool[,] matrix)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                matrix[row, col] = Random.value > emptySpaceProbability;
            }
        }
    }

    private void GenerateCheckerboardPattern(bool[,] matrix)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                matrix[row, col] = (row + col) % 2 == 0 && Random.value > emptySpaceProbability;
            }
        }
    }

    private void GenerateZigZagPattern(bool[,] matrix)
    {
        for (int row = 0; row < rows; row++)
        {
            int offset = row % 2;
            for (int col = 0; col < columns; col++)
            {
                matrix[row, col] = (col + offset) % 2 == 0 && Random.value > emptySpaceProbability;
            }
        }
    }

    private void GenerateDiamondPattern(bool[,] matrix)
    {
        int centerX = columns / 2;
        int centerY = rows / 2;
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float distanceFromCenter = Mathf.Abs(col - centerX) + Mathf.Abs(row - centerY);
                matrix[row, col] = distanceFromCenter <= Mathf.Min(rows, columns) / 2 
                                 && Random.value > emptySpaceProbability;
            }
        }
    }

    private void GenerateWallsPattern(bool[,] matrix)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                bool isWall = col == 0 || col == columns - 1 || row == 0 || row == rows - 1;
                matrix[row, col] = isWall && Random.value > emptySpaceProbability;
            }
        }
    }

    private void GeneratePyramidPattern(bool[,] matrix)
    {
        int maxHeight = Mathf.Min(rows, columns / 2);
        
        for (int row = 0; row < maxHeight; row++)
        {
            int startCol = row;
            int endCol = columns - row - 1;
            
            for (int col = startCol; col <= endCol; col++)
            {
                matrix[rows - row - 1, col] = Random.value > emptySpaceProbability;
            }
        }
    }

    private void GenerateSpiralPattern(bool[,] matrix)
    {
        int top = 0, bottom = rows - 1, left = 0, right = columns - 1;
        
        while (top <= bottom && left <= right)
        {
            // Superior
            for (int col = left; col <= right; col++)
                matrix[top, col] = Random.value > emptySpaceProbability;
            top++;

            // Derecha
            for (int row = top; row <= bottom; row++)
                matrix[row, right] = Random.value > emptySpaceProbability;
            right--;

            if (top <= bottom)
            {
                // Inferior
                for (int col = right; col >= left; col--)
                    matrix[bottom, col] = Random.value > emptySpaceProbability;
                bottom--;
            }

            if (left <= right)
            {
                // Izquierda
                for (int row = bottom; row >= top; row--)
                    matrix[row, left] = Random.value > emptySpaceProbability;
                left++;
            }
        }
    }

    private void SpawnBlocks(bool[,] matrix)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (matrix[row, col])
                {
                    // Seleccionar tipo de bloque basado en probabilidades
                    BlockType selectedBlock = SelectBlockType();
                    if (selectedBlock != null)
                    {
                        Vector3 position = new Vector3(
                            startPos.x + col * (blockWidth + spacing),
                            startPos.y - row * (blockHeight + spacing),
                            0
                        );

                        GameObject block = Instantiate(selectedBlock.prefab, position, Quaternion.identity);
                        currentBlocks.Add(block);
                        
                        // Configurar componentes del bloque si es necesario
                        Block blockComponent = block.GetComponent<Block>();
                        if (blockComponent != null)
                        {
                            blockComponent.points = selectedBlock.points;
                            blockComponent.isIndestructible = selectedBlock.isIndestructible;
                        }
                    }
                }
            }
        }
    }

    private BlockType SelectBlockType()
    {
        float totalProbability = 0;
        foreach (BlockType block in blockTypes)
        {
            totalProbability += block.probability;
        }

        float random = Random.Range(0, totalProbability);
        float currentProbability = 0;

        foreach (BlockType block in blockTypes)
        {
            currentProbability += block.probability;
            if (random <= currentProbability)
            {
                return block;
            }
        }

        return blockTypes[0]; // Bloque por defecto
    }

    public void NextLevel()
    {
        currentLevel++;
        GenerateLevel();
    }

    public void RestartGame()
    {
        currentLevel = 1;
        GenerateLevel();
    }
}