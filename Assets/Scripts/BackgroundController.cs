using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private Material backgroundMaterial;
    public Color mainColor = Color.blue;
    public Color secondaryColor = Color.black;
    public float gridSize = 10f;
    public float scrollSpeed = 1f;
    public float emissionIntensity = 2f;

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        backgroundMaterial = renderer.material;
        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        backgroundMaterial.SetColor("_MainColor", mainColor);
        backgroundMaterial.SetColor("_SecondaryColor", secondaryColor);
        backgroundMaterial.SetFloat("_GridSize", gridSize);
        backgroundMaterial.SetVector("_ScrollSpeed", new Vector4(0, scrollSpeed, 0, 0));
        backgroundMaterial.SetFloat("_EmissionIntensity", emissionIntensity);
    }
}
