using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonBlock : MonoBehaviour
{
    [Header("Color Settings")]
    public Color mainColor = Color.white;
    public Color emissionColor = Color.cyan;
    public float emissionIntensity = 2f;
    
    [Header("Pulse Settings")]
    public bool enablePulse = true;
    public float pulseSpeed = 2f;
    public float minIntensity = 1f;
    public float maxIntensity = 3f;
    
    private Material material;
    private float time;

    void Start()
    {
        // Crear una instancia única del material
        material = new Material(GetComponent<Renderer>().sharedMaterial);
        GetComponent<Renderer>().material = material;
        
        // Configurar colores iniciales
        material.SetColor("_Color", mainColor);
        material.EnableKeyword("_EMISSION");
        UpdateEmissionIntensity(emissionIntensity);
    }

    void Update()
    {
        if (enablePulse)
        {
            time += Time.deltaTime * pulseSpeed;
            float pulse = Mathf.Lerp(minIntensity, maxIntensity, 
                (Mathf.Sin(time) + 1f) / 2f);
            UpdateEmissionIntensity(pulse);
        }
    }

    void UpdateEmissionIntensity(float intensity)
    {
        material.SetColor("_EmissionColor", emissionColor * intensity);
    }
}
