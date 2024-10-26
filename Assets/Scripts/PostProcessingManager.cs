using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private float bloomIntensity = 1.5f;
    [SerializeField] private float bloomThreshold = 1.0f;
    [SerializeField] private float chromaticAberrationIntensity = 0.3f;
    [SerializeField] private float vignetteIntensity = 0.4f;

    private PostProcessVolume volume;
    private PostProcessProfile profile;

    void Awake()
    {
        // Asegúrate de que tienes el Volume
        volume = gameObject.GetComponent<PostProcessVolume>();
        if (volume == null)
        {
            volume = gameObject.AddComponent<PostProcessVolume>();
        }

        // Crea un nuevo perfil
        profile = ScriptableObject.CreateInstance<PostProcessProfile>();
        
        // Configura el volumen
        volume.isGlobal = true;
        volume.profile = profile;

        // Configura los efectos
        SetupBloom();
        SetupChromaticAberration();
        SetupVignette();
        
        // Configura la cámara
        SetupCamera();
    }

    void SetupCamera()
    {
        // Obtén la cámara principal
        Camera mainCamera = Camera.main;
        
        // Añade el Post Process Layer si no existe
        PostProcessLayer layer = mainCamera.GetComponent<PostProcessLayer>();
        if (layer == null)
        {
            layer = mainCamera.gameObject.AddComponent<PostProcessLayer>();
        }
        
        // Configura el layer
        layer.volumeLayer = LayerMask.GetMask("Everything");
        layer.volumeTrigger = mainCamera.transform;
    }

    void SetupBloom()
    {
        var bloom = profile.AddSettings<Bloom>();
        bloom.enabled.value = true;
        bloom.intensity.value = bloomIntensity;
        bloom.threshold.value = bloomThreshold;
        bloom.softKnee.value = 0.5f;
        bloom.clamp.value = 65472f;
        bloom.diffusion.value = 7f;
        bloom.anamorphicRatio.value = 0f;
        bloom.color.value = Color.white;
        bloom.fastMode.value = false;
    }

    void SetupChromaticAberration()
    {
        var chromaticAberration = profile.AddSettings<ChromaticAberration>();
        chromaticAberration.enabled.value = true;
        chromaticAberration.intensity.value = chromaticAberrationIntensity;
        chromaticAberration.fastMode.value = false;
    }

    void SetupVignette()
    {
        var vignette = profile.AddSettings<Vignette>();
        vignette.enabled.value = true;
        vignette.intensity.value = vignetteIntensity;
        vignette.smoothness.value = 0.2f;
        vignette.roundness.value = 1f;
        vignette.color.value = Color.black;
    }

    // Métodos públicos para ajustar efectos en tiempo real
    public void UpdateBloomIntensity(float intensity)
    {
        var bloom = profile.GetSetting<Bloom>();
        bloom.intensity.value = intensity;
    }

    public void UpdateChromaticAberration(float intensity)
    {
        var chromaticAberration = profile.GetSetting<ChromaticAberration>();
        chromaticAberration.intensity.value = intensity;
    }

    public void UpdateVignetteIntensity(float intensity)
    {
        var vignette = profile.GetSetting<Vignette>();
        vignette.intensity.value = intensity;
    }
}
