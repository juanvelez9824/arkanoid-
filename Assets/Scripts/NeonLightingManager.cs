using UnityEngine;

public class NeonLightingManager : MonoBehaviour
{
    [System.Serializable]
    public class NeonLight
    {
        public string lightName = "NeonLight";
        public Color lightColor = Color.cyan;
        public float intensity = 2f;
        public float range = 10f;
        public Vector3 position = Vector3.zero;
        public bool pulse = true;
        public float pulseSpeed = 1f;
        public float pulseIntensity = 0.2f;
        
        [HideInInspector]
        public Light lightComponent;
    }

    [Header("Main Light Settings")]
    [SerializeField] private Light mainDirectionalLight;
    [SerializeField] private Color mainLightColor = new Color(0.1f, 0.1f, 0.2f);
    [SerializeField] private float mainLightIntensity = 0.3f;

    [Header("Neon Lights")]
    [SerializeField] private NeonLight[] neonLights;

    [Header("Global Settings")]
    [SerializeField] private bool automaticallyCreateLights = true;
    [SerializeField] private Transform neonLightsContainer;

    void Start()
    {
        SetupLighting();
    }

    void SetupLighting()
    {
        SetupMainLight();
        SetupNeonLights();
    }

    void SetupMainLight()
    {
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.intensity = mainLightIntensity;
            mainDirectionalLight.color = mainLightColor;
        }
    }

    void SetupNeonLights()
    {
        if (neonLightsContainer == null && automaticallyCreateLights)
        {
            neonLightsContainer = new GameObject("NeonLights").transform;
            neonLightsContainer.SetParent(transform);
        }

        foreach (NeonLight neonLight in neonLights)
        {
            if (neonLight.lightComponent == null && automaticallyCreateLights)
            {
                // Crear nuevo objeto de luz
                GameObject lightObj = new GameObject(neonLight.lightName);
                lightObj.transform.SetParent(neonLightsContainer);
                lightObj.transform.localPosition = neonLight.position;

                // Añadir y configurar el componente Light
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = neonLight.lightColor;
                light.intensity = neonLight.intensity;
                light.range = neonLight.range;
                light.shadows = LightShadows.Soft;

                neonLight.lightComponent = light;
            }
        }
    }

    void Update()
    {
        UpdateNeonLights();
    }

    void UpdateNeonLights()
    {
        foreach (NeonLight neonLight in neonLights)
        {
            if (neonLight.lightComponent != null && neonLight.pulse)
            {
                float pulseValue = Mathf.Sin(Time.time * neonLight.pulseSpeed);
                neonLight.lightComponent.intensity = neonLight.intensity + 
                    (pulseValue * neonLight.pulseIntensity);
            }
        }
    }

    // Método para añadir luces en tiempo de ejecución
    public void AddNeonLight(Vector3 position, Color color, float intensity = 2f, float range = 10f)
    {
        GameObject lightObj = new GameObject($"NeonLight_{neonLights.Length + 1}");
        lightObj.transform.SetParent(neonLightsContainer);
        lightObj.transform.position = position;

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.Soft;

        // Expandir el array de luces
        System.Array.Resize(ref neonLights, neonLights.Length + 1);
        neonLights[neonLights.Length - 1] = new NeonLight
        {
            lightName = lightObj.name,
            lightColor = color,
            intensity = intensity,
            range = range,
            position = position,
            lightComponent = light
        };
    }

    // Método para configuración rápida de luces comunes en Arkanoid
    public void SetupDefaultArkanoidLights()
    {
        // Luz para la plataforma
        AddNeonLight(new Vector3(0, -2, -1), Color.cyan, 2f, 8f);
        
        // Luces laterales
        AddNeonLight(new Vector3(-5, 0, -1), Color.magenta, 1.5f, 6f);
        AddNeonLight(new Vector3(5, 0, -1), Color.magenta, 1.5f, 6f);
        
        // Luz superior
        AddNeonLight(new Vector3(0, 5, -1), Color.green, 1.8f, 10f);
    }
}
