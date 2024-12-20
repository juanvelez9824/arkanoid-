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
        public float powerUpDropChance = 0.2f;
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

    [Header("Límites Personalizados")]
    public Transform boundaryFrame; // Referencia al objeto que define el área de juego
    public bool showBoundaryGizmos = true; // Para visualizar el área en el editor
    
    [System.Serializable]
    public class PowerUpSettings
    {
        public GameObject powerUpPrefab;
        public PowerUpType type;
        public float spawnProbability = 1f;
    }
    
    [Header("Dimensiones")]
    public int minRows = 3;
    public int maxRows = 6;
    public int minColumns = 5;
    public int maxColumns = 12;
    public float blockWidth = 1.5f;
    public float blockHeight = 0.5f;
    public float spacing = 0.1f; // Espacio entre bloques

    [Header("Power-Ups")]
    public PowerUpSettings[] availablePowerUps;
    public float globalPowerUpChance = 0.3f; // Probabilidad global de que aparezca un power-up
    public float minPowerUpSpawnHeight = 2f;
    
    [Header("Configuración")]
    public float emptySpaceProbability = 0.1f;
    
    [Header("Dificultad")]
    public bool increaseDifficultyWithLevel = true;
    public float difficultyMultiplier = 1.1f;
    public int currentLevel = 1;
    
    private int rows;
    private int columns;
    private List<GameObject> currentBlocks = new List<GameObject>();
    private Bounds boundaryBounds;
    private Vector2 startPos;

    void Start()
    {
        if (boundaryFrame == null)
        {
            Debug.LogError("¡No se ha asignado el marco de límites! Por favor, asigna un objeto para definir el área de juego.");
            return;
        }
        
        CalculateBoundaryBounds();
        GenerateLevel();
    }

    private void CalculateBoundaryBounds()
    {
        // Obtener los límites del marco
        Renderer frameRenderer = boundaryFrame.GetComponent<Renderer>();
        if (frameRenderer != null)
        {
            boundaryBounds = frameRenderer.bounds;
        }
        else
        {
            // Si no tiene Renderer, usar el tamaño del objeto
            boundaryBounds = new Bounds(boundaryFrame.position, boundaryFrame.localScale);
        }
    }

    private void AdjustLevelDimensions()
    {
        // Calcular el espacio disponible dentro del marco
        float availableWidth = boundaryBounds.size.x;
        float availableHeight = boundaryBounds.size.y;

        // Calcular el máximo número de columnas que caben en el ancho disponible
        int maxPossibleColumns = Mathf.FloorToInt(availableWidth / (blockWidth + spacing));
        maxColumns = Mathf.Min(maxColumns, maxPossibleColumns);

        // Calcular el máximo número de filas que caben en el alto disponible
        int maxPossibleRows = Mathf.FloorToInt(availableHeight / (blockHeight + spacing));
        maxRows = Mathf.Min(maxRows, maxPossibleRows);

        // Calcular la posición inicial para centrar los bloques en el marco
        float levelWidth = columns * (blockWidth + spacing) - spacing;
        float levelHeight = rows * (blockHeight + spacing) - spacing;

        startPos = new Vector2(
            boundaryBounds.min.x + (availableWidth - levelWidth) * 0.5f,
            boundaryBounds.max.y - (availableHeight - levelHeight) * 0.5f
        );
    }

    public void GenerateLevel()
    {
        if (boundaryFrame == null) return;

        ClearLevel();
        AdjustDifficultyForLevel();
        
        // Establecer dimensiones del nivel
        rows = Random.Range(minRows, maxRows + 1);
        columns = Random.Range(minColumns, maxColumns + 1);

        // Ajustar dimensiones según los límites del marco
        AdjustLevelDimensions();

        if (randomizePattern)
        {
            pattern = (LevelPattern)Random.Range(0, System.Enum.GetValues(typeof(LevelPattern)).Length);
        }

        bool[,] levelMatrix = GenerateLevelMatrix();
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
            globalPowerUpChance = Mathf.Max(0.1f, globalPowerUpChance - (0.02f * currentLevel));
            // Aumentar probabilidad de bloques más resistentes
            foreach (BlockType block in blockTypes)
            {
                if (block.isIndestructible)
                {
                    block.probability = Mathf.Min(0.3f, block.probability + (0.02f * currentLevel));
                }
            }
       
              if (availablePowerUps != null)
            {
                foreach (PowerUpSettings powerUp in availablePowerUps)
                {
                    switch (powerUp.type)
                    {
                        case PowerUpType.ExplodingBlocks:
                        case PowerUpType.ExtraLife:
                            // Power-ups más poderosos son más raros pero aumentan con el nivel
                            powerUp.spawnProbability = Mathf.Min(1f, powerUp.spawnProbability + (0.05f * currentLevel));
                            break;
                        default:
                            // Power-ups básicos mantienen probabilidad constante
                            break;
                    }
                }
       
       
       
       
       
       
             } }
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
                    BlockType selectedBlock = SelectBlockType();
                    if (selectedBlock != null)
                    {
                        Vector3 position = new Vector3(
                            startPos.x + col * (blockWidth + spacing),
                            startPos.y - row * (blockHeight + spacing),
                            boundaryFrame.position.z
                        );

                        // Verificar si el bloque está dentro de los límites
                        if (IsPositionWithinBounds(position))
                        {
                            GameObject block = Instantiate(selectedBlock.prefab, position, Quaternion.identity);
                            currentBlocks.Add(block);
                            
                            Block blockComponent = block.GetComponent<Block>();
                            if (blockComponent != null)
                            {
                                blockComponent.points = selectedBlock.points;
                                blockComponent.isIndestructible = selectedBlock.isIndestructible;

                                // Configurar el power-up para este bloque
                                SetupBlockPowerUp(blockComponent, selectedBlock, position);
                            }
                        }
                    }
                }
            }
        }
    }

     private void SetupBlockPowerUp(Block block, BlockType blockType, Vector3 position)
    {
        // Solo considerar power-ups si el bloque está por encima de la altura mínima
        if (position.y < boundaryBounds.min.y + minPowerUpSpawnHeight)
            return;

        // Verificar si este bloque debería tener un power-up
        if (Random.value < globalPowerUpChance * blockType.powerUpDropChance)
        {
            PowerUpSettings selectedPowerUp = SelectPowerUp();
            if (selectedPowerUp != null)
            {
                // Guardar la información del power-up en el componente Block
                block.containsPowerUp = true;
                block.powerUpPrefab = selectedPowerUp.powerUpPrefab;
                block.powerUpType = selectedPowerUp.type;
            }
        }
    }

     private PowerUpSettings SelectPowerUp()
    {
        if (availablePowerUps == null || availablePowerUps.Length == 0)
            return null;

        float totalProbability = 0;
        foreach (PowerUpSettings powerUp in availablePowerUps)
        {
            totalProbability += powerUp.spawnProbability;
        }

        float random = Random.Range(0, totalProbability);
        float currentProbability = 0;

        foreach (PowerUpSettings powerUp in availablePowerUps)
        {
            currentProbability += powerUp.spawnProbability;
            if (random <= currentProbability)
            {
                return powerUp;
            }
        }

        return availablePowerUps[0];
    }

    private bool IsPositionWithinBounds(Vector3 position)
    {
        // Verificar si la posición (incluyendo el tamaño del bloque) está dentro del marco
        return position.x >= boundaryBounds.min.x &&
               position.x + blockWidth <= boundaryBounds.max.x &&
               position.y - blockHeight >= boundaryBounds.min.y &&
               position.y <= boundaryBounds.max.y;
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

    private void OnDrawGizmos()
    {
        if (showBoundaryGizmos && boundaryFrame != null)
        {
            // Dibujar el área de juego en el editor
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundaryBounds.center, boundaryBounds.size);
        }
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