using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }

    [Header("Ball Settings")]
    public float initialSpeed = 10f;
    public float maxSpeed = 20f;
    public float minSpeed = 8f;
    public float currentSpeed;

    [Header("Movement Controls")]
    [SerializeField] private float maxYVelocity = 15f;
    [SerializeField] private float minYVelocity = 5f;
    [SerializeField] private float velocityVariation = 0.1f;
    [SerializeField] private float minBounceAngle = 0.4f;
    [SerializeField] private float randomDirectionChange = 0.2f;
    
    [Header("Visual Effects")]
    [SerializeField] private BallEffectsManager effectsManager;
    [SerializeField] private Material ballMaterial;
    private Color originalColor;
    
    [Header("Power-up Effects")]
    [SerializeField] private float powerUpDuration = 10f;
    [SerializeField] private Color powerUpColor = Color.cyan;
    [SerializeField] private float speedPowerUpMultiplier = 1.5f;
    private bool isPowerUpActive = false;
    private Coroutine currentPowerUpCoroutine;

    [Header("References")]
    [SerializeField] private GameObject bottomPaddle;
    [SerializeField] private GameObject topPaddle;
    
    // Components
    private Rigidbody rb;
    private Vector3 initialPosition;
    private float lastVerticalChangeTime;
    private float verticalChangeTimeout = 0.5f;
    private bool isAttachedToPaddle = true;
    private Vector3 offsetFromPaddle;
    private PaddleController.PaddleType lastHitPaddle;

    [Header("Screen Shake")]
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.1f;
    private Camera mainCamera;
    private Vector3 originalCameraPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        Physics.defaultMaxDepenetrationVelocity = 5f;
        Physics.bounceThreshold = 0.2f;
    }

    private void Start()
    {
        // Inicializar componentes
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        lastVerticalChangeTime = Time.time;
        currentSpeed = initialSpeed;
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.position;
        
        // Configurar Rigidbody
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.angularDrag = 0f;
        rb.drag = 0f;
        
        // Inicializar efectos visuales
        if (ballMaterial == null)
            ballMaterial = GetComponent<Renderer>().material;
        originalColor = ballMaterial.color;
        
        // Configurar posición inicial
        if (bottomPaddle != null)
        {
            offsetFromPaddle = transform.position - bottomPaddle.transform.position;
        }
        else
        {
            Debug.LogError("¡Paddle inferior no asignado en BallController!");
        }

        AttachBallToPaddle();
    }

    public void AttachBallToPaddle()
    {
        isAttachedToPaddle = true;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        
        if (bottomPaddle != null)
        {
            transform.position = bottomPaddle.transform.position + offsetFromPaddle;
        }
        
        currentSpeed = initialSpeed;
        lastHitPaddle = PaddleController.PaddleType.Bottom;
    }

    public void LaunchBall()
    {
        if (isAttachedToPaddle)
        {
            isAttachedToPaddle = false;
            rb.isKinematic = false;
            
            float randomX = Random.Range(-0.4f, 0.4f);
            Vector3 direction = new Vector3(randomX, 1, 0).normalized;
            currentSpeed = initialSpeed;
            rb.velocity = direction * currentSpeed;
            lastVerticalChangeTime = Time.time;
            
            if (effectsManager != null)
            {
                effectsManager.PlayCollisionEffect(transform.position, CollisionType.Paddle); 
            }
        }
    }

    private void HandleBallMovement()
    {
        Vector3 velocity = rb.velocity;
        
        // Añadir variación aleatoria
        if (Random.value < randomDirectionChange * Time.deltaTime)
        {
            velocity += new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                0
            );
        }
        
        // Corregir velocidad
        if (Mathf.Abs(velocity.magnitude - currentSpeed) > 0.1f)
        {
            velocity = velocity.normalized * currentSpeed;
        }
        
        // Evitar movimiento vertical perpetuo
        if (Mathf.Abs(velocity.x) < 0.1f)
        {
            velocity.x = 0.2f * Mathf.Sign(Random.Range(-1f, 1f));
        }
        
        // Aplicar límites de velocidad vertical
        if (Mathf.Abs(velocity.y) < minYVelocity)
        {
            if (Time.time - lastVerticalChangeTime > verticalChangeTimeout)
            {
                float sign = (velocity.y >= 0) ? 1 : -1;
                Vector3 newVelocity = velocity.normalized;
                newVelocity.y = minBounceAngle * sign;
                newVelocity = newVelocity.normalized * currentSpeed;
                velocity = newVelocity;
                lastVerticalChangeTime = Time.time;
            }
        }
        else if (Mathf.Abs(velocity.y) > maxYVelocity)
        {
            velocity.y = Mathf.Sign(velocity.y) * maxYVelocity;
            velocity = velocity.normalized * currentSpeed;
        }

        rb.velocity = velocity;
    }

    private void FixedUpdate()
    {
        if (!isAttachedToPaddle)
        {
            HandleBallMovement();
        }
    }

    private void Update()
    {
        if (isAttachedToPaddle)
        {
            if (bottomPaddle != null)
            {
                transform.position = bottomPaddle.transform.position + offsetFromPaddle;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                LaunchBall();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {   
        if (isAttachedToPaddle) return;
        
        if (collision.gameObject.CompareTag("Paddle"))
        {
            HandlePaddleCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            HandleWallCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Block"))
        {
            HandleBlockCollision(collision);
        }

        currentSpeed = Mathf.Clamp(rb.velocity.magnitude, minSpeed, maxSpeed);
    }

    private void HandlePaddleCollision(Collision collision)
    {
        PaddleController paddle = collision.gameObject.GetComponent<PaddleController>();
        if (paddle != null && paddle.GetPaddleType() != lastHitPaddle)
        {
            Vector3 bounceDirection = GetBounceDirection(collision, paddle.GetPaddleType());
            
            float speedVariation = 1f + Random.Range(-velocityVariation, velocityVariation);
            currentSpeed = Mathf.Clamp(currentSpeed * speedVariation, minSpeed, maxSpeed);
            
            rb.velocity = bounceDirection * currentSpeed;
            lastVerticalChangeTime = Time.time;
            lastHitPaddle = paddle.GetPaddleType();
            
            if (effectsManager != null)
            {
                effectsManager.PlayCollisionEffect(collision.contacts[0].point, CollisionType.Paddle);
            }

            StartCoroutine(ShakeScreen(shakeIntensity, shakeDuration));
        }
    }

    private void HandleWallCollision(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 direction = Vector3.Reflect(rb.velocity.normalized, normal);
        
        direction += new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f),
            0
        ).normalized;
        
        if (Mathf.Abs(direction.y) < minBounceAngle)
        {
            direction.y = minBounceAngle * Mathf.Sign(direction.y);
            direction = direction.normalized;
        }
        
        rb.velocity = direction * currentSpeed;
        
        if (Mathf.Abs(direction.y) >= minYVelocity)
        {
            lastVerticalChangeTime = Time.time;
        }
        
        if (effectsManager != null)
        {
            effectsManager.PlayCollisionEffect(collision.contacts[0].point, CollisionType.Wall);
        }
    }

    private void HandleBlockCollision(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 direction = Vector3.Reflect(rb.velocity.normalized, normal);
        
        direction += new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f),
            0
        ).normalized;
        
        if (Mathf.Abs(direction.y) < minBounceAngle)
        {
            direction.y = minBounceAngle * Mathf.Sign(direction.y);
            direction = direction.normalized;
        }
        
        float speedVariation = 1f + Random.Range(-velocityVariation, velocityVariation);
        currentSpeed = Mathf.Clamp(currentSpeed * speedVariation, minSpeed, maxSpeed);
        
        rb.velocity = direction * currentSpeed;
        lastVerticalChangeTime = Time.time;
        
        if (effectsManager != null)
        {
            effectsManager.PlayCollisionEffect(collision.contacts[0].point, CollisionType.Block);
        }
        
        StartCoroutine(ShakeScreen(shakeIntensity * 0.5f, shakeDuration));
    }

     public void StartPowerUpEffect(PowerUpType type)
    {
        if (currentPowerUpCoroutine != null)
        {
            StopCoroutine(currentPowerUpCoroutine);
        }
        currentPowerUpCoroutine = StartCoroutine(PowerUpRoutine(type));
    }

    private Vector3 GetBounceDirection(Collision collision, PaddleController.PaddleType paddleType)
    {
        Vector3 hitPoint = collision.contacts[0].point;
        Vector3 paddleCenter = collision.gameObject.transform.position;

        float paddleWidth = collision.collider.bounds.size.x;
        float offsetX = (hitPoint.x - paddleCenter.x) / (paddleWidth * 0.5f);
        float bounceX = offsetX * 1.5f;
        
        float bounceY = (paddleType == PaddleController.PaddleType.Bottom) ? 1 : -1;
        Vector3 bounceDirection = new Vector3(bounceX, bounceY, 0).normalized;

        if (Mathf.Abs(bounceDirection.y) < minBounceAngle)
        {
            bounceDirection.y = minBounceAngle * Mathf.Sign(bounceDirection.y);
            bounceDirection = bounceDirection.normalized;
        }

        return bounceDirection;
    }

    private IEnumerator PowerUpRoutine(PowerUpType type)
    {
        isPowerUpActive = true;

        if (effectsManager != null)
        {
            effectsManager.StartPowerUpEffect(powerUpColor);
        }

        switch (type)
        {
            case PowerUpType.BallSpeed:
                float originalSpeed = currentSpeed;
                float originalMaxSpeed = maxSpeed;
                
                currentSpeed *= speedPowerUpMultiplier;
                maxSpeed *= speedPowerUpMultiplier;
                
                yield return new WaitForSeconds(powerUpDuration);
                
                currentSpeed = originalSpeed;
                maxSpeed = originalMaxSpeed;
                break;
        }

        if (effectsManager != null)
        {
            effectsManager.StopPowerUpEffect();
        }

        isPowerUpActive = false;
        currentPowerUpCoroutine = null;
    }

    private IEnumerator ShakeScreen(float intensity, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.transform.position = originalCameraPosition;
    }

    public void ResetBall()
    {
        if (currentPowerUpCoroutine != null)
        {
            StopCoroutine(currentPowerUpCoroutine);
            currentPowerUpCoroutine = null;
        }

         if (effectsManager != null)
        {
            effectsManager.StopPowerUpEffect(); // Changed from StopAllEffects
            if (effectsManager.powerUpEffect != null)
            {
                effectsManager.powerUpEffect.Stop();
            }
            if (effectsManager.collisionEffect != null)
            {
                effectsManager.collisionEffect.Stop();
            }
        }

        rb.velocity = Vector3.zero;
        transform.position = initialPosition;
        transform.localScale = Vector3.one;
        currentSpeed = initialSpeed;
        isPowerUpActive = false;
        
        AttachBallToPaddle();
    }

    public bool IsPowerUpActive()
    {
        return isPowerUpActive;
    }

    public void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}