using UnityEngine;

public class NeonFrameManager : MonoBehaviour
{
    [Header("Frame References")]
    [SerializeField] private GameObject topBorder;
    [SerializeField] private GameObject bottomBorder;
    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;
    
    [Header("Visual Settings")]
    [SerializeField] private Material frameMaterial;
    [SerializeField] private float glowIntensity = 2f;
    [SerializeField] private Color primaryColor = Color.cyan;
    [SerializeField] private Color secondaryColor = Color.magenta;
    
    [Header("Animation")]
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float colorChangeSpeed = 0.5f;
    
    private Material[] borderMaterials;
    
    void Start()
    {
        InitializeFrame();
    }
    
    void InitializeFrame()
    {
        // Crear materiales individuales para cada borde
        borderMaterials = new Material[4];
        GameObject[] borders = { topBorder, bottomBorder, leftBorder, rightBorder };
        
        for (int i = 0; i < borders.Length; i++)
        {
            if (borders[i] != null)
            {
                borderMaterials[i] = new Material(frameMaterial);
                borders[i].GetComponent<Renderer>().material = borderMaterials[i];
                SetupMaterial(borderMaterials[i]);
            }
        }
    }
    
    void SetupMaterial(Material mat)
    {
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", primaryColor * glowIntensity);
    }
    
    void Update()
    {
        AnimateFrame();
    }
    
    void AnimateFrame()
    {
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float colorLerp = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
        
        Color currentColor = Color.Lerp(primaryColor, secondaryColor, colorLerp);
        
        foreach (Material mat in borderMaterials)
        {
            if (mat != null)
            {
                mat.SetColor("_EmissionColor", currentColor * (glowIntensity * (1f + pulse * 0.2f)));
            }
        }
    }
}
