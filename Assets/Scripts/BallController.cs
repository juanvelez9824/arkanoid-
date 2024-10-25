using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }

    public float initialSpeed = 10f;
    public float maxSpeed = 20f;
    public float minSpeed = 8f;
    
    [SerializeField] private float maxYVelocity = 15f;
    [SerializeField] private float minYVelocity = 5f;
    [SerializeField] private float velocityVariation = 0.1f;
    
    private Rigidbody rb;
    private Vector3 initialPosition;
    private float minBounceAngle = 0.4f;
    private float lastVerticalChangeTime;
    private float verticalChangeTimeout = 0.5f;

    [SerializeField] private GameObject bottomPaddle; // Paddle inferior
    [SerializeField] private GameObject topPaddle;    // Paddle superior
    private bool isAttachedToPaddle = true;
    private Vector3 offsetFromPaddle;
    private PaddleController.PaddleType lastHitPaddle;
    
    private float currentSpeed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        Physics.defaultMaxDepenetrationVelocity = 5f;
        Physics.bounceThreshold = 0.2f;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        lastVerticalChangeTime = Time.time;
        currentSpeed = initialSpeed;
        
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.angularDrag = 0f;
        rb.drag = 0f;
        
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
        }
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

    private void HandleBallMovement()
    {
        Vector3 velocity = rb.velocity;
        
        if (Mathf.Abs(velocity.magnitude - currentSpeed) > 0.1f)
        {
            velocity = velocity.normalized * currentSpeed;
            rb.velocity = velocity;
        }
        
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

        currentSpeed = Mathf.Clamp(rb.velocity.magnitude, minSpeed, maxSpeed);
    }

    private void HandlePaddleCollision(Collision collision)
    {
        PaddleController paddle = collision.gameObject.GetComponent<PaddleController>();
        if (paddle != null)
        {
            // Verificar que la pelota no rebote en el mismo paddle dos veces seguidas
            if (paddle.GetPaddleType() != lastHitPaddle)
            {
                Vector3 bounceDirection = GetBounceDirection(collision, paddle.GetPaddleType());
                
                float speedVariation = 1f + Random.Range(-velocityVariation, velocityVariation);
                currentSpeed = Mathf.Clamp(currentSpeed * speedVariation, minSpeed, maxSpeed);
                
                rb.velocity = bounceDirection * currentSpeed;
                lastVerticalChangeTime = Time.time;
                lastHitPaddle = paddle.GetPaddleType();
                
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.paddleHit);
                }
            }
        }
    }

    private void HandleWallCollision(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 direction = Vector3.Reflect(rb.velocity.normalized, normal);
        
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

    private Vector3 GetBounceDirection(Collision collision, PaddleController.PaddleType paddleType)
    {
        Vector3 hitPoint = collision.contacts[0].point;
        Vector3 paddleCenter = collision.gameObject.transform.position;

        float paddleWidth = collision.collider.bounds.size.x;
        float offsetX = (hitPoint.x - paddleCenter.x) / (paddleWidth * 0.5f);
        float bounceX = offsetX * 1.5f;
        
        // Dirección del rebote basada en el tipo de paddle
        float bounceY = (paddleType == PaddleController.PaddleType.Bottom) ? 1 : -1;
        Vector3 bounceDirection = new Vector3(bounceX, bounceY, 0).normalized;

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
        AttachBallToPaddle();
    }
}
