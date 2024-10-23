using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }

    public float initialSpeed = 10f;
    public float maxSpeed = 20f;
    public float minSpeed = 8f; // Aumentado para mantener más momentum
    
    [SerializeField] private float maxYVelocity = 15f;
    [SerializeField] private float minYVelocity = 5f; // Aumentado para evitar movimiento horizontal
    [SerializeField] private float velocityVariation = 0.1f; // Reducido para más consistencia
    
    private Rigidbody rb;
    private Vector3 initialPosition;
    private float minBounceAngle = 0.4f; // Aumentado para evitar rebotes muy horizontales
    private float lastVerticalChangeTime;
    private float verticalChangeTimeout = 0.5f; // Reducido para corregir más rápido

    [SerializeField] private GameObject paddle;
    private bool isAttachedToPaddle = true;
    private Vector3 offsetFromPaddle;
    
    // Nuevo: para mantener velocidad constante
    private float currentSpeed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        // Asegurar que la física sea consistente
        Physics.defaultMaxDepenetrationVelocity = 2f;
        Physics.bounceThreshold = 0.2f;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        lastVerticalChangeTime = Time.time;
        currentSpeed = initialSpeed;
        
        // Configurar el Rigidbody para movimiento más suave
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.angularDrag = 0f;
        rb.drag = 0f; // Eliminar fricción
        
        if (paddle != null)
        {
            offsetFromPaddle = transform.position - paddle.transform.position;
        }
        else
        {
            Debug.LogError("¡Paddle no asignado en BallController!");
        }

        AttachBallToPaddle();
    }
    public void AttachBallToPaddle()
    {
        isAttachedToPaddle = true;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        
        if (paddle != null)
        {
            transform.position = paddle.transform.position + offsetFromPaddle;
        }
        
        currentSpeed = initialSpeed; // Reiniciar velocidad al adherirse al paddle
    }

    public void LaunchBall()
    {
        if (isAttachedToPaddle)
        {
            isAttachedToPaddle = false;
            rb.isKinematic = false;
            
            // Lanzamiento más controlado
            float randomX = Random.Range(-0.3f, 0.3f);
            Vector3 direction = new Vector3(randomX, 1, 0).normalized;
            currentSpeed = initialSpeed;
            rb.velocity = direction * currentSpeed;
            lastVerticalChangeTime = Time.time;
        }
    }

    private void FixedUpdate() // Cambiado a FixedUpdate para física más precisa
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
            if (paddle != null)
            {
                transform.position = paddle.transform.position + offsetFromPaddle;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                LaunchBall();
            }
        }
    }

    private void HandleBallMovement()
    {
        Vector3 velocity = rb.velocity;
        
        // Mantener velocidad constante
        if (Mathf.Abs(velocity.magnitude - currentSpeed) > 0.1f)
        {
            velocity = velocity.normalized * currentSpeed;
            rb.velocity = velocity;
        }
        
        // Corregir movimiento muy horizontal
        if (Mathf.Abs(velocity.y) < minYVelocity)
        {
            if (Time.time - lastVerticalChangeTime > verticalChangeTimeout)
            {
                float sign = (velocity.y >= 0) ? 1 : -1;
                Vector3 newVelocity = velocity.normalized;
                newVelocity.y = minBounceAngle * sign;
                newVelocity = newVelocity.normalized * currentSpeed;
                rb.velocity = newVelocity;
                lastVerticalChangeTime = Time.time;
            }
        }
        else
        {
            lastVerticalChangeTime = Time.time;
        }

        // Limitar velocidad vertical
        if (Mathf.Abs(velocity.y) > maxYVelocity)
        {
            velocity.y = Mathf.Sign(velocity.y) * maxYVelocity;
            velocity = velocity.normalized * currentSpeed;
            rb.velocity = velocity;
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

        // Mantener velocidad dentro de los límites
        currentSpeed = Mathf.Clamp(rb.velocity.magnitude, minSpeed, maxSpeed);
    }

    private void HandlePaddleCollision(Collision collision)
    {
        Vector3 bounceDirection = GetBounceDirection(collision);
        
        // Pequeña variación en la velocidad para mantener el juego interesante
        float speedVariation = 1f + Random.Range(-velocityVariation, velocityVariation);
        currentSpeed = Mathf.Clamp(currentSpeed * speedVariation, minSpeed, maxSpeed);
        
        rb.velocity = bounceDirection * currentSpeed;
        lastVerticalChangeTime = Time.time;
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.paddleHit);
        }
    }

    private void HandleWallCollision(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 direction = Vector3.Reflect(rb.velocity.normalized, normal);
        
        // Asegurar rebote con ángulo mínimo
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
    }

    private void HandleBlockCollision(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 direction = Vector3.Reflect(rb.velocity.normalized, normal);
        
        if (Mathf.Abs(direction.y) < minBounceAngle)
        {
            direction.y = minBounceAngle * Mathf.Sign(direction.y);
            direction = direction.normalized;
        }
        
        float speedVariation = 1f + Random.Range(-velocityVariation, velocityVariation);
        currentSpeed = Mathf.Clamp(currentSpeed * speedVariation, minSpeed, maxSpeed);
        
        rb.velocity = direction * currentSpeed;
        lastVerticalChangeTime = Time.time;
    }

    private Vector3 GetBounceDirection(Collision collision)
    {
        Vector3 hitPoint = collision.contacts[0].point;
        Vector3 paddleCenter = collision.gameObject.transform.position;

        // Hacer el rebote más sensible a la posición del impacto
        float paddleWidth = collision.collider.bounds.size.x;
        float offsetX = (hitPoint.x - paddleCenter.x) / (paddleWidth * 0.5f);
        float bounceX = offsetX * 1.5f;
        
        Vector3 bounceDirection = new Vector3(bounceX, 1, 0).normalized;

        if (Mathf.Abs(bounceDirection.y) < minBounceAngle)
        {
            bounceDirection.y = minBounceAngle * Mathf.Sign(bounceDirection.y);
            bounceDirection = bounceDirection.normalized;
        }

        return bounceDirection;
    }

    public void ResetBall()
    {
        rb.velocity = Vector3.zero;
        transform.position = initialPosition;
        currentSpeed = initialSpeed;
        LaunchBall();
    }
}

