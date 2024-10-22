using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }
    public float initialSpeed = 10f;
    public float maxSpeed = 20f;
    private Rigidbody rb;
    private Vector3 initialPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        LaunchBall();
    }

    public void LaunchBall()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, 0).normalized;
        rb.velocity = randomDirection * initialSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.paddleHit);
        if (collision.gameObject.CompareTag("Block"))
        {
            GameManager.Instance.AddScore(10);  // Añadir puntos
            AudioManager.Instance.PlaySFX(AudioManager.Instance.blockDestroy);  // Sonido al romper el bloque
        }

        // Limitar la velocidad de la pelota para evitar que se vuelva ingobernable
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    public void ResetBall()
    {
        rb.velocity = Vector3.zero;
        transform.position = initialPosition;
        LaunchBall();
    }
}
