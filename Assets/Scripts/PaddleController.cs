using UnityEngine;
using System;

public class PaddleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 40f;       
    [SerializeField] private float maxX = 7.5f;
    
    [Header("Paddle Type")]
    [SerializeField] private PaddleType paddleType = PaddleType.Bottom;
    
    [Header("Input Keys")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    
    [Header("References")]
    [SerializeField] private BallController ballController;

    [Header("Power-up Settings")]
    [SerializeField] private float defaultSize = 1f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private float powerUpDuration = 10f;
    [SerializeField] private AnimationCurve powerUpSizeCurve;

    private Renderer paddleRenderer;
    private float currentPowerUpTime;
    private bool isPoweredUp;

    public bool canMove = true;
    
    // Enum para identificar el tipo de paddle
    public enum PaddleType
    {
        Bottom,
        Top
    }

    private void Awake()
    {
        paddleRenderer = GetComponent<Renderer>();
        if (paddleRenderer == null)
        {
            Debug.LogError("No se encontró el componente Renderer en el paddle!");
        }

        // Configurar teclas según el tipo de paddle
        SetupInputKeys();
        ResetPaddleState();
    }

    private void SetupInputKeys()
    {
        switch (paddleType)
        {
            case PaddleType.Bottom:
                moveLeftKey = KeyCode.A;
                moveRightKey = KeyCode.D;
                break;
            case PaddleType.Top:
                moveLeftKey = KeyCode.LeftArrow;
                moveRightKey = KeyCode.RightArrow;
                break;
        }
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovement();
        }
        UpdatePowerUpState();
    }

    private void HandleMovement()
    {   
        // Movimiento directo basado en input
        float moveDirection = 0;
        
        if (Input.GetKey(moveLeftKey))
        {
            moveDirection = -1;
        }
        else if (Input.GetKey(moveRightKey))
        {
            moveDirection = 1;
        }

        // Aplicar movimiento directo
        float movement = moveDirection * speed * Time.deltaTime;
        float newX = Mathf.Clamp(transform.position.x + movement, -maxX, maxX);
        
        // Actualizar posición directamente
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    public void ApplyPowerUp(float sizeMultiplier, Color newColor, float duration = 0f)
    {
        if (duration <= 0) duration = powerUpDuration;

        StartCoroutine(AnimatePowerUpSize(sizeMultiplier));
        
        paddleRenderer.material.color = newColor;
        currentPowerUpTime = duration;
        isPoweredUp = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.powerUp);
        }
        else
        {
            Debug.LogWarning("AudioManager no encontrado!");
        }

        OnPowerUpApplied?.Invoke(sizeMultiplier, newColor, duration);
    }

    private System.Collections.IEnumerator AnimatePowerUpSize(float targetSize)
    {
        float startSize = transform.localScale.x;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (powerUpSizeCurve != null)
            {
                t = powerUpSizeCurve.Evaluate(t);
            }
            
            float currentSize = Mathf.Lerp(startSize, defaultSize * targetSize, t);
            transform.localScale = new Vector3(currentSize, transform.localScale.y, transform.localScale.z);
            
            yield return null;
        }
    }

    private void UpdatePowerUpState()
    {
        if (!isPoweredUp) return;

        currentPowerUpTime -= Time.deltaTime;
        
        if (currentPowerUpTime <= 0)
        {
            ResetPaddleState();
        }
    }

    private void ResetPaddleState()
    {
        StartCoroutine(AnimatePowerUpSize(1f));
        paddleRenderer.material.color = defaultColor;
        isPoweredUp = false;
        currentPowerUpTime = 0;
        
        OnPowerUpExpired?.Invoke();
    }

    public event Action<float, Color, float> OnPowerUpApplied;
    public event Action OnPowerUpExpired;

    public void ResetPaddle()
    {
        transform.position = new Vector3(0, transform.position.y, 0);
        ResetPaddleState();
    }

    public PaddleType GetPaddleType()
    {
        return paddleType;
    }
}