using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BackgroundScaler : MonoBehaviour
{
    private Image backgroundImage;
    
    [Header("Background Settings")]
    [SerializeField] private bool maintainAspectRatio = true;
    [SerializeField] private bool fillScreen = true;
    
    void Awake()
    {
        backgroundImage = GetComponent<Image>();
        SetupBackground();
    }

    void SetupBackground()
    {
        if (backgroundImage != null)
        {
            // Configurar la imagen para llenar el panel
            backgroundImage.type = Image.Type.Sliced;
            
            if (fillScreen)
            {
                // Hacer que la imagen llene toda la pantalla
                backgroundImage.rectTransform.anchorMin = Vector2.zero;
                backgroundImage.rectTransform.anchorMax = Vector2.one;
                backgroundImage.rectTransform.sizeDelta = Vector2.zero;
                backgroundImage.rectTransform.anchoredPosition = Vector2.zero;
            }
            
            if (maintainAspectRatio)
            {
                backgroundImage.preserveAspect = true;
            }
        }
    }
}
