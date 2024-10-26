using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{   
    [Header("Block Settings")]
    [SerializeField] public int hitPoints = 2;
    [SerializeField] public Color damagedColor;
    [SerializeField] public int points = 100;
    [SerializeField] public bool isIndestructible = false;
    
    [Header("Visual Effects")]
    [SerializeField] private float flickerDuration = 0.3f;
    [SerializeField] private int flickerCount = 3;
    [SerializeField] private Color flickerColor = Color.white;
    [SerializeField] private int shatterPieces = 8;
    [SerializeField] private float explosionForce = 300f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float pieceDestroyDelay = 2f;
    [SerializeField] private GameObject shatterPiecePrefab; // Prefab simple para las piezas
    [SerializeField] private Material shatterMaterial; // Material para las piezas
    
    [Header("Neon Effects")]
    [SerializeField] private Color emissionColor = Color.cyan;
    [SerializeField] private float emissionIntensity = 2f;
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minEmissionIntensity = 1f;
    [SerializeField] private float maxEmissionIntensity = 3f;
    
    private Renderer blockRenderer;
    private bool isBeingDestroyed = false;
    private AudioManager audioManager;
    private Color originalColor;
    private Material originalMaterial;
    private Vector3 originalScale;
  

     public bool containsPowerUp = false;
    public GameObject powerUpPrefab;
    public PowerUpType powerUpType;

    private void Start()
    {   
        blockRenderer = GetComponent<Renderer>();

        originalMaterial = new Material(blockRenderer.sharedMaterial);
        blockRenderer.material = originalMaterial;
    
         // Configurar emisión
        originalMaterial.EnableKeyword("_EMISSION");
        originalMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        originalColor = originalMaterial.color;
         originalScale = transform.localScale;

        if (enablePulse)
        {
        StartCoroutine(PulseEffect());
        }
        
        
        if (blockRenderer == null)
        {
            Debug.LogError($"[{gameObject.name}] No se encontró el Renderer en el bloque");
        }
        
        // Obtener referencia al AudioManager al inicio
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogWarning($"[{gameObject.name}] No se encontró el AudioManager");
        }

        // Registrar el bloque con el LevelManager si no es indestructible
        if (!isIndestructible && LevelManager.Instance != null)
        {
            // El tag se necesita para que el LevelManager pueda encontrar los bloques al inicio
            gameObject.tag = "Block";
        }
    }

    private void OnDestroy()
    {
        // No generar power-up si el bloque se destruye al cambiar de nivel
        if (!gameObject.scene.isLoaded) return;

        if (containsPowerUp && powerUpPrefab != null)
        {
            SpawnPowerUp();
        }
    }

      private void SpawnPowerUp()
    {
        Vector3 spawnPosition = transform.position;
        Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (isBeingDestroyed || isIndestructible) return;

        if (collision.gameObject.CompareTag("Ball"))
        {   
            StartCoroutine(ImpactEffect(collision.contacts[0].point));
            TakeDamage();
        }
    }

    private IEnumerator ImpactEffect(Vector3 hitPoint)
    {
        // Efecto de onda expansiva en el punto de impacto
        if (blockRenderer != null)
        {
            Color currentEmission = originalMaterial.GetColor("_EmissionColor");
            originalMaterial.SetColor("_EmissionColor", flickerColor * emissionIntensity * 3);
        
            yield return new WaitForSeconds(0.1f);
        
            originalMaterial.SetColor("_EmissionColor", currentEmission);
        }
    }

    private void TakeDamage()
    {
        hitPoints--;

        if (hitPoints > 0)
        {
            StartCoroutine(FlickerEffect());
            StartCoroutine(ShakeEffect());
        }
        else
        {
            StartDestroySequence();
        }
    }

     private IEnumerator FlickerEffect()
    {
        if (blockRenderer == null) yield break;

        float flickerInterval = flickerDuration / (flickerCount * 2);
        Color originalEmission = originalMaterial.GetColor("_EmissionColor");
        
        for (int i = 0; i < flickerCount; i++)
        {
            originalMaterial.SetColor("_EmissionColor", flickerColor * emissionIntensity * 2);
            yield return new WaitForSeconds(flickerInterval);
            originalMaterial.SetColor("_EmissionColor", originalEmission);
            yield return new WaitForSeconds(flickerInterval);
        }
    }

    
    private IEnumerator ShakeEffect()
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;
        float shakeDuration = 0.2f;
        float shakeIntensity = 0.05f;

        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * shakeIntensity;
            float y = originalPosition.y + Random.Range(-1f, 1f) * shakeIntensity;
            float z = originalPosition.z;

            transform.position = new Vector3(x, y, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }

    private void StartDestroySequence()
    {
        if (isBeingDestroyed) return;
        
        isBeingDestroyed = true;
        
        // Reproducir sonido primero
        PlayDestructionSound();
       
        
        // Luego iniciar la destrucción física
        StartCoroutine(DestroyBlockSequence());
    }

    private IEnumerator DestroyBlockSequence()
    {
        // Desactivar visual y físicamente el bloque
        if (blockRenderer != null)
        {
            blockRenderer.enabled = false;
        }

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        CreateShatterEffect();

        // Añadir puntos
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(points);
        }

        // Notificar al LevelManager antes de destruir el objeto
        if (!isIndestructible && LevelManager.Instance != null)
        {
            LevelManager.Instance.RemoveBlock(gameObject);
        }

        // Esperar un frame para asegurar que el sonido se reproduce
        yield return new WaitForEndOfFrame();

        // Destruir el objeto
        Destroy(gameObject);
    }

    private void SetupShatterPieceMaterial(GameObject piece)
{
    Renderer pieceRenderer = piece.GetComponent<Renderer>();
    if (pieceRenderer != null)
    {
        Material pieceMaterial = new Material(originalMaterial);
        pieceRenderer.material = pieceMaterial;
        pieceMaterial.EnableKeyword("_EMISSION");
        pieceMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
    }
}

    private void CreateShatterEffect()
    {
      if (shatterPiecePrefab == null)
    {
        CreateDefaultShatterPieces();
        return;
    }

    Vector3 blockCenter = transform.position;
    
    for (int i = 0; i < shatterPieces; i++)
    {
        GameObject piece = Instantiate(shatterPiecePrefab, blockCenter, Random.rotation);
        
        SetupShatterPieceMaterial(piece);

        float scale = Random.Range(0.2f, 0.4f);
        piece.transform.localScale = originalScale * scale;

        Rigidbody rb = piece.GetComponent<Rigidbody>();
        if (rb == null) rb = piece.AddComponent<Rigidbody>();
        
        Vector3 randomDir = Random.insideUnitSphere;
        rb.AddForce(randomDir * explosionForce);
        rb.AddTorque(randomDir * explosionForce);

        Destroy(piece, pieceDestroyDelay);
    }
    }

     private void CreateDefaultShatterPieces()
    {
        // Crear cubos simples si no hay prefab
        for (int i = 0; i < shatterPieces; i++)
        {
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piece.transform.position = transform.position;
            piece.transform.rotation = Random.rotation;
            
            float scale = Random.Range(0.1f, 0.3f);
            piece.transform.localScale = originalScale * scale;

            Renderer pieceRenderer = piece.GetComponent<Renderer>();
            pieceRenderer.material = originalMaterial;
            pieceRenderer.material.color = originalColor;

            Rigidbody rb = piece.AddComponent<Rigidbody>();
            Vector3 randomDir = Random.insideUnitSphere;
            rb.AddForce(randomDir * explosionForce);
            rb.AddTorque(randomDir * explosionForce);

            Destroy(piece, pieceDestroyDelay);
        }
    }

    private void OnValidate()
    {
        // Asegurarse de que los valores tengan sentido
        flickerDuration = Mathf.Max(0.1f, flickerDuration);
        flickerCount = Mathf.Max(1, flickerCount);
        shatterPieces = Mathf.Clamp(shatterPieces, 4, 20);
        explosionForce = Mathf.Max(100f, explosionForce);
        pieceDestroyDelay = Mathf.Max(0.5f, pieceDestroyDelay);
    }


    private void PlayDestructionSound()
    {
        // Intentar obtener el AudioManager si no lo teníamos
        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;
        }

        if (audioManager != null && audioManager.blockDestroy != null)
        {
            audioManager.PlaySFX(audioManager.blockDestroy);
            Debug.Log($"[{gameObject.name}] Reproduciendo sonido de destrucción");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] No se pudo reproducir el sonido: AudioManager o blockDestroy es null");
            if (audioManager == null)
            {
                Debug.LogError("AudioManager.Instance es null");
            }
            else if (audioManager.blockDestroy == null)
            {
                Debug.LogError("audioManager.blockDestroy es null");
            }
        }
    }

    private IEnumerator PulseEffect()
    {
    float time = 0;
    
    while (enabled)
    {
        time += Time.deltaTime * pulseSpeed;
        float pulseIntensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, 
            (Mathf.Sin(time) + 1f) / 2f);
        
        originalMaterial.SetColor("_EmissionColor", emissionColor * pulseIntensity);
        
        yield return null;
    }
    }

    public void SetBlockColor(Color mainColor, Color neonColor)
    {
    if (originalMaterial != null)
    {
        originalMaterial.color = mainColor;
        emissionColor = neonColor;
        originalMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
    }
    }




    public void ForceDestroy()
    {
        StartDestroySequence();
    }
}