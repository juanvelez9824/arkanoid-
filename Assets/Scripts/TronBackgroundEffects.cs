using UnityEngine;

public class TronBackgroundEffects : MonoBehaviour
{
    [Header("Partículas Flotantes")]
    public ParticleSystem backgroundParticles;
    public int particleCount = 100;
    public float particleSpeed = 0.5f;
    public Vector3 boxSize = new Vector3(10f, 10f, 2f);

    [Header("Niebla Volumétrica")]
    public Material fogMaterial;
    public float fogDensity = 0.1f;
    public Color fogColor = new Color(0, 1, 1, 0.1f); // Color cyan semi-transparente

    [Header("Grid Tron")]
    public Material gridMaterial;
    public float gridSize = 1f;
    public float gridGlowIntensity = 1.5f;
    public Color gridColor = new Color(0, 1, 1); // Color cyan

    [Header("Ondas de Energía")]
    public float waveSpeed = 1f;
    public float waveAmplitude = 0.5f;
    public float waveFrequency = 2f;

    private void Start()
    {
        SetupFloatingParticles();
        SetupVolumetricFog();
        SetupTronGrid();
    }

    private void SetupFloatingParticles()
    {
        var main = backgroundParticles.main;
        main.maxParticles = particleCount;
        
        var emission = backgroundParticles.emission;
        emission.rateOverTime = particleCount / 5f;

        var shape = backgroundParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = boxSize;

        var velocityOverLifetime = backgroundParticles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.speedModifier = particleSpeed;
    }

    private void SetupVolumetricFog()
    {
        if (fogMaterial != null)
        {
            fogMaterial.SetFloat("_Density", fogDensity);
            fogMaterial.SetColor("_FogColor", fogColor);
            
            // Shader properties para el efecto de niebla volumétrica
            fogMaterial.SetVector("_NoiseScale", new Vector4(0.1f, 0.1f, 0.1f, 1f));
            fogMaterial.SetFloat("_NoiseSpeed", 0.5f);
        }
    }

    private void SetupTronGrid()
    {
        if (gridMaterial != null)
        {
            gridMaterial.SetFloat("_GridSize", gridSize);
            gridMaterial.SetFloat("_GlowIntensity", gridGlowIntensity);
            gridMaterial.SetColor("_GridColor", gridColor);
        }
    }

    private void Update()
    {
        UpdateEnergyWaves();
    }

    private void UpdateEnergyWaves()
    {
        float time = Time.time * waveSpeed;
        float wave = Mathf.Sin(time * waveFrequency) * waveAmplitude;
        
        // Aplicar la onda a los materiales
        if (gridMaterial != null)
        {
            gridMaterial.SetFloat("_WaveOffset", wave);
        }
    }
}

