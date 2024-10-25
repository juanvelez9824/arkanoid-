using UnityEngine;

public class Block : MonoBehaviour
{   
    [SerializeField]
    public int hitPoints = 2;
    private Renderer blockrenderer;
    public Color damagedColor;
    public int points = 100;
    public bool isIndestructible = false;
    
    private bool isBeingDestroyed = false;
    private AudioManager audioManager;

    private void Start()
    {
        blockrenderer = GetComponent<Renderer>();
        if (blockrenderer == null)
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

    public void OnCollisionEnter(Collision collision)
    {
        if (isBeingDestroyed || isIndestructible) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        hitPoints--;

        if (hitPoints > 0)
        {
            if (blockrenderer != null && blockrenderer.material != null)
            {
                blockrenderer.material.color = damagedColor;
            }
        }
        else
        {
            StartDestroySequence();
        }
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

    private System.Collections.IEnumerator DestroyBlockSequence()
    {
        // Desactivar visual y físicamente el bloque
        if (blockrenderer != null)
        {
            blockrenderer.enabled = false;
        }

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

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

    public void ForceDestroy()
    {
        StartDestroySequence();
    }
}