using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    // Referencia a los objetos generados
    private GameObject frameContainer;

    private void OnEnable()
    {
        #if UNITY_EDITOR
        // Solo generar en el editor si no es una instancia de prefab
        if (!PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            EditorApplication.delayCall += () =>
            {
                if (this != null && gameObject != null)
                {
                    GenerateFrame();
                }
            };
        }
        #endif
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        // No ejecutar si es parte de un prefab
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            return;

        if (!gameObject.activeInHierarchy) 
            return;

        EditorApplication.delayCall += () =>
        {
            if (this != null && gameObject != null)
            {
                GenerateFrame();
            }
        };
    }
    #endif

    public void GenerateFrame()
    {
        // Si es una instancia de prefab, no regenerar
        #if UNITY_EDITOR
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            return;
        #endif

        EnsureContainer();
        CreateFrameParts();
        CreateCorners();
        if (addBevelEffect)
        {
            AddBevelToFrame();
        }
    }

    void EnsureContainer()
    {
        // Limpiar el contenedor anterior si existe
        if (frameContainer != null)
        {
            #if UNITY_EDITOR
            DestroyImmediate(frameContainer);
            #else
            Destroy(frameContainer);
            #endif
        }

        // Crear nuevo contenedor
        frameContainer = new GameObject("FrameParts");
        frameContainer.transform.SetParent(transform);
        frameContainer.transform.localPosition = Vector3.zero;
        frameContainer.transform.localRotation = Quaternion.identity;
        frameContainer.transform.localScale = Vector3.one;
    }
    
    void CreateFrameParts()
    {
        CreateBorder("TopBorder", new Vector3(0, gameAreaSize.y/2, 0), new Vector3(gameAreaSize.x + frameWidth*2, frameWidth, frameDepth));
        CreateBorder("BottomBorder", new Vector3(0, -gameAreaSize.y/2, 0), new Vector3(gameAreaSize.x + frameWidth*2, frameWidth, frameDepth));
        CreateBorder("LeftBorder", new Vector3(-gameAreaSize.x/2, 0, 0), new Vector3(frameWidth, gameAreaSize.y, frameDepth));
        CreateBorder("RightBorder", new Vector3(gameAreaSize.x/2, 0, 0), new Vector3(frameWidth, gameAreaSize.y, frameDepth));
    }
    
    void CreateBorder(string name, Vector3 position, Vector3 scale)
    {
        GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
        border.name = name;
        border.transform.SetParent(frameContainer.transform, false);
        border.transform.localPosition = position;
        border.transform.localScale = scale;
        
        if (frameMaterial != null)
        {
            border.GetComponent<Renderer>().sharedMaterial = frameMaterial;
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
        corner.transform.SetParent(frameContainer.transform, false);
        corner.transform.localPosition = position;
        corner.transform.localScale = new Vector3(cornerSize, cornerSize, frameDepth);
        
        if (frameMaterial != null)
        {
            corner.GetComponent<Renderer>().sharedMaterial = frameMaterial;
        }
    }
    
    void AddBevelToFrame()
    {
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
        bevel.transform.SetParent(frameContainer.transform, false);
        bevel.transform.localPosition = position;
        bevel.transform.localScale = scale;
        
        if (frameMaterial != null)
        {
            bevel.GetComponent<Renderer>().sharedMaterial = frameMaterial;
        }
    }
}