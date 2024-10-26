using UnityEngine;
using System.Collections;

public enum PowerUpType 
{
    PaddleSize,
    BallSpeed,
    ExtraLife,
    ExplodingBlocks
}

public class PowerUp : MonoBehaviour 
{
    [Header("Settings")]
    [SerializeField] private PowerUpType type;
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float rotationSpeed = 90f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem collectionEffect;
    [SerializeField] private TrailRenderer powerUpTrail;
    [SerializeField] private Color powerUpColor = Color.yellow;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseIntensity = 0.2f;
    
    [Header("Power-up Effects")]
    [SerializeField] private float paddleSizeMultiplier = 1.5f;
    [SerializeField] private float ballSpeedMultiplier = 1.3f;
    [SerializeField] private float explosionRadius = 2f;

    private Material powerUpMaterial;
    private float initialScale;
    private bool isCollected = false;

    private void Start()
    {
        // Inicializar efectos visuales
        powerUpMaterial = GetComponent<Renderer>().material;
        powerUpMaterial.color = powerUpColor;
        initialScale = transform.localScale.x;
        
        if (powerUpTrail == null)
            powerUpTrail = GetComponent<TrailRenderer>();
            
        if (powerUpTrail != null)
        {
            powerUpTrail.startColor = powerUpColor;
            powerUpTrail.endColor = new Color(powerUpColor.r, powerUpColor.g, powerUpColor.b, 0f);
        }
    }

    private void Update() 
    {
        if (isCollected) return;

        // Mover el power-up hacia abajo
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        
        // Rotar el power-up
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Efecto de pulso
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        transform.localScale = Vector3.one * initialScale * pulse;
        
        // Destruir si sale de la pantalla
        if (transform.position.y < -10f) 
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Paddle") && !isCollected) 
        {
            isCollected = true;
            StartCoroutine(CollectPowerUp(other.gameObject));
        }
    }

    private IEnumerator CollectPowerUp(GameObject paddle)
    {
        // Efectos visuales de recolección
        if (collectionEffect != null)
        {
            collectionEffect.Play();
        }

        // Desactivar el renderer y el collider
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        
        if (powerUpTrail != null)
        {
            powerUpTrail.enabled = false;
        }

        // Aplicar el power-up
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ActivatePowerUp(type);
        }
        
        ApplyPowerUp(paddle);

        // Esperar a que terminen los efectos
        yield return new WaitForSeconds(1f);
        
        Destroy(gameObject);
    }

    private void ApplyPowerUp(GameObject paddle) 
    {
        PaddleController paddleController = paddle.GetComponent<PaddleController>();
        
        switch (type) 
        {
            case PowerUpType.PaddleSize:
                if (paddleController != null) 
                {
                    paddleController.ApplyPowerUp(paddleSizeMultiplier, powerUpColor);
                }
                break;

            case PowerUpType.BallSpeed:
                if (BallController.Instance != null) 
                {
                    BallController.Instance.StartPowerUpEffect(type);
                }
                break;

            case PowerUpType.ExtraLife:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddLife();
                }
                break;

            case PowerUpType.ExplodingBlocks:
                StartCoroutine(TriggerBlockExplosion(paddle.transform.position));
                break;
        }
    }

    private IEnumerator TriggerBlockExplosion(Vector3 paddlePosition) 
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(paddlePosition, explosionRadius);
        
        foreach (Collider obj in nearbyObjects) 
        {
            if (obj.CompareTag("Block")) 
            {
                StartCoroutine(ExplodeBlock(obj.gameObject));
                yield return new WaitForSeconds(0.1f); // Pequeño retraso entre explosiones
            }
        }
    }

    private IEnumerator ExplodeBlock(GameObject block) 
    {
        if (block != null) 
        {
            // Efectos de explosión
            ParticleSystem explosion = block.GetComponent<ParticleSystem>();
            if (explosion != null) 
            {
                explosion.Play();
                yield return new WaitForSeconds(0.5f);
            }
            
            // Añadir puntos si es necesario
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(100); // Puntos por explosión
            }
            
            Destroy(block);
        }
    }
}
