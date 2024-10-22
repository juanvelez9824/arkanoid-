using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power-up Settings")]
    public float fallSpeed = 5f;
    public float duration = 10f;
    public PowerUpType type;
    
    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Paddle"))
        {
            ApplyPowerUp(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("DeathZone"))
        {
            Destroy(gameObject);
        }
    }
    
    private void ApplyPowerUp(GameObject paddle)
    {
        switch (type)
        {
            case PowerUpType.WidePaddle:
                paddle.GetComponent<Paddle>().IncreaseSize(1.5f);
                StartCoroutine(ResetPaddleSize(paddle, 1.5f));
                break;
            // Add more power-up types here
        }
        
        // Play power-up sound
        AudioManager.Instance?.PlaySound("powerUpCollect");
    }
    
    private System.Collections.IEnumerator ResetPaddleSize(GameObject paddle, float multiplier)
    {
        yield return new WaitForSeconds(duration);
        paddle.GetComponent<Paddle>().IncreaseSize(1f/multiplier);
    }
}

public enum PowerUpType
{
    WidePaddle,
    ExtraLife,
    FastBall
}
