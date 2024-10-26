using UnityEngine;

[ExecuteInEditMode]
public class RetroFrameGenerator : MonoBehaviour
{
    [Header("Frame Dimensions")]
    [SerializeField] private float frameWidth = 0.5f;
    [SerializeField] private float frameDepth = 0.1f;
    [SerializeField] private float cornerSize = 1f;
    
    [Header("Visual Style")]
    [SerializeField] private Material frameMaterial;
    [SerializeField] private bool addBevelEffect = true;
    [SerializeField] private int bevelSegments = 3;
    
    [Header("Game Area")]
    [SerializeField] private Vector2 gameAreaSize = new Vector2(10f, 15f);
    
    private GameObject[] frameParts;
    
    void OnValidate()
    {
        GenerateFrame();
    }
    
    public void GenerateFrame()
    {
        CleanupOldFrame();
        CreateFrameParts();
        CreateCorners();
        if (addBevelEffect)
        {
            AddBevelToFrame();
        }
    }
    
    void CleanupOldFrame()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    
    void CreateFrameParts()
    {
        // Crear los bordes principales
        CreateBorder("TopBorder", new Vector3(0, gameAreaSize.y/2, 0), new Vector3(gameAreaSize.x + frameWidth*2, frameWidth, frameDepth));
        CreateBorder("BottomBorder", new Vector3(0, -gameAreaSize.y/2, 0), new Vector3(gameAreaSize.x + frameWidth*2, frameWidth, frameDepth));
        CreateBorder("LeftBorder", new Vector3(-gameAreaSize.x/2, 0, 0), new Vector3(frameWidth, gameAreaSize.y, frameDepth));
        CreateBorder("RightBorder", new Vector3(gameAreaSize.x/2, 0, 0), new Vector3(frameWidth, gameAreaSize.y, frameDepth));
    }
    
    void CreateBorder(string name, Vector3 position, Vector3 scale)
    {
        GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
        border.name = name;
        border.transform.SetParent(transform);
        border.transform.localPosition = position;
        border.transform.localScale = scale;
        
        if (frameMaterial != null)
        {
            border.GetComponent<Renderer>().material = frameMaterial;
        }
    }
    
    void CreateCorners()
    {
        Vector3[] cornerPositions = {
            new Vector3(-gameAreaSize.x/2 - frameWidth/2, gameAreaSize.y/2 + frameWidth/2, 0),
            new Vector3(gameAreaSize.x/2 + frameWidth/2, gameAreaSize.y/2 + frameWidth/2, 0),
            new Vector3(-gameAreaSize.x/2 - frameWidth/2, -gameAreaSize.y/2 - frameWidth/2, 0),
            new Vector3(gameAreaSize.x/2 + frameWidth/2, -gameAreaSize.y/2 - frameWidth/2, 0)
        };
        
        for (int i = 0; i < cornerPositions.Length; i++)
        {
            CreateCorner($"Corner_{i}", cornerPositions[i]);
        }
    }
    
    void CreateCorner(string name, Vector3 position)
    {
        GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Cube);
        corner.name = name;
        corner.transform.SetParent(transform);
        corner.transform.localPosition = position;
        corner.transform.localScale = new Vector3(cornerSize, cornerSize, frameDepth);
        
        if (frameMaterial != null)
        {
            corner.GetComponent<Renderer>().material = frameMaterial;
        }
    }
    
    void AddBevelToFrame()
    {
        // Añadir segmentos pequeños para crear efecto biselado
        float bevelSize = frameWidth / bevelSegments;
        
        for (int i = 0; i < bevelSegments; i++)
        {
            float offset = (frameWidth / 2) - (bevelSize * i);
            
            CreateBevelSegment($"TopBevel_{i}", 
                new Vector3(0, gameAreaSize.y/2 + offset, frameDepth/2),
                new Vector3(gameAreaSize.x + frameWidth*2 - bevelSize*2*i, bevelSize, bevelSize));
                
            CreateBevelSegment($"BottomBevel_{i}", 
                new Vector3(0, -gameAreaSize.y/2 - offset, frameDepth/2),
                new Vector3(gameAreaSize.x + frameWidth*2 - bevelSize*2*i, bevelSize, bevelSize));
        }
    }
    
    void CreateBevelSegment(string name, Vector3 position, Vector3 scale)
    {
        GameObject bevel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bevel.name = name;
        bevel.transform.SetParent(transform);
        bevel.transform.localPosition = position;
        bevel.transform.localScale = scale;
        
        if (frameMaterial != null)
        {
            bevel.GetComponent<Renderer>().material = frameMaterial;
        }
    }
}
