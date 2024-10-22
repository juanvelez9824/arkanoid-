using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Settings")]
    public float initialSpeed = 8f;
    public float maxSpeed = 15f;
    public float speedIncrease = 0.1f;
    
    private Rigidbody2D rb;
    private bool inPlay;
    private Transform paddle;
    private Vector2 paddleToBallVector;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        paddle = GameObject.FindGameObjectWithTag("Paddle").transform;
        paddleToBallVector = transform.position - paddle.position;
        inPlay = false;
    }
    
    void Update()
    {
        if (!inPlay)
        {
            // Stay with paddle
            transform.position = paddle.position + (Vector3)paddleToBallVector;
            
            // Launch ball
            if (Input.GetButtonDown("Jump"))
            {
                inPlay = true;
                rb.velocity = Vector2.up * initialSpeed;
                AudioManager.Instance?.PlaySound("launch");
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            // Increase speed slightly after each block hit
            Vector2 velocity = rb.velocity;
            float speed = velocity.magnitude;
            speed = Mathf.Min(speed + speedIncrease, maxSpeed);
            rb.velocity = velocity.normalized * speed;
            AudioManager.Instance?.PlaySound("blockHit");
        }
        
        else
        {
        AudioManager.Instance?.PlaySound("bounce");
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone"))
        {
            GameManager.Instance.LoseLife();
            ResetBall();
        }
    }
    
    public void ResetBall()
    {
        rb.velocity = Vector2.zero;
        inPlay = false;
    }
}


