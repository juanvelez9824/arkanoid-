using UnityEngine;

public class Paddle : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float paddleWidth = 2f;
    
    private float minX, maxX;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        CalculateBounds();
    }
    
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 position = transform.position;
        
        position.x += horizontalInput * moveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        
        transform.position = position;
    }
    
    private void CalculateBounds()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float worldHeight = mainCamera.orthographicSize * 2;
        float worldWidth = worldHeight * screenRatio;
        
        minX = -worldWidth/2 + paddleWidth/2;
        maxX = worldWidth/2 - paddleWidth/2;
    }
    
    public void IncreaseSize(float multiplier)
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= multiplier;
        transform.localScale = newScale;
        paddleWidth *= multiplier;
        CalculateBounds();
    }
}
