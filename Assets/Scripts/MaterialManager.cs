using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [Header("Material References")]
    [SerializeField] private Material blockMaterial;
    [SerializeField] private Material paddleMaterial;
    [SerializeField] private Material ballMaterial;
    
    [Header("Glow Settings")]
    [SerializeField] private Color blockGlowColor = Color.cyan;
    [SerializeField] private float glowIntensity = 1.5f;
    
    void Start()
    {
        SetupMaterials();
    }
    
    void SetupMaterials()
    {
        // Configurar material de los bloques
        blockMaterial.EnableKeyword("_EMISSION");
        blockMaterial.SetColor("_EmissionColor", blockGlowColor * glowIntensity);
        
        // Configurar material de la pelota
        ballMaterial.EnableKeyword("_EMISSION");
        ballMaterial.SetColor("_EmissionColor", Color.white * glowIntensity);
        
        // Configurar material de la pala
        paddleMaterial.EnableKeyword("_EMISSION");
        paddleMaterial.SetColor("_EmissionColor", Color.white * glowIntensity);
    }
}
