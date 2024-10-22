using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Block Settings")]
    public int hitPoints = 1;
    public int scoreValue = 100;
    public bool dropsPowerUp = false;
    public GameObject powerUpPrefab;
    
    [Header("Visual Feedback")]
    public Sprite[] damageSprites;
    
    private SpriteRenderer spriteRenderer;
    private int currentHitPoints;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHitPoints = hitPoints;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            TakeHit();
        }
    }
    
    private void TakeHit()
    {
        currentHitPoints--;
        
        // Update visual feedback
        if (damageSprites.Length > 0 && currentHitPoints > 0)
        {
            int spriteIndex = hitPoints - currentHitPoints - 1;
            if (spriteIndex >= 0 && spriteIndex < damageSprites.Length)
            {
                spriteRenderer.sprite = damageSprites[spriteIndex];
            }
        }
        
        if (currentHitPoints <= 0)
        {
            DestroyBlock();
        }
        
        // Play hit sound
        AudioManager.Instance?.PlaySound("blockHit");
    }
    
    private void DestroyBlock()
    {
        GameManager.Instance.AddScore(scoreValue);
        GameManager.Instance.BlockDestroyed();
        
        if (dropsPowerUp && powerUpPrefab != null)
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}
