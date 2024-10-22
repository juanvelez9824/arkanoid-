using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public Vector3[] blockPositions;
        public int[] blockTypes; // 0 = normal, 1 = resistente
    }

    [SerializeField] private GameObject[] blockPrefabs;
    [SerializeField] private LevelData[] levels;
    private List<GameObject> currentBlocks = new List<GameObject>();

    public void LoadLevel(int levelIndex)
    {
        ClearLevel();
        
        LevelData level = levels[levelIndex];
        for (int i = 0; i < level.blockPositions.Length; i++)
        {
            GameObject block = Instantiate(
                blockPrefabs[level.blockTypes[i]],
                level.blockPositions[i],
                Quaternion.identity
            );
            currentBlocks.Add(block);
        }
    }

    private void ClearLevel()
    {
        foreach (GameObject block in currentBlocks)
        {
            if (block != null)
                Destroy(block);
        }
        currentBlocks.Clear();
    }

    public bool IsLevelComplete()
    {
        return currentBlocks.Count == 0;
    }
}