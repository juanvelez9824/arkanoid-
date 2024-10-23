using UnityEngine;
using System;

public class PaddleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 40f;        // Aumentado significativamente
    [SerializeField] private float maxX = 7.5f;

    [Header("References")]
    [SerializeField] private BallController ballController;

    [Header("Power-up Settings")]
    [SerializeField] private float defaultSize = 1f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private float powerUpDuration = 10f;

    private Renderer paddleRenderer;
    private float currentPowerUpTime;
    private bool isPoweredUp;

    private void Awake()
    {
        paddleRenderer = GetComponent<Renderer>();
        if (paddleRenderer == null)
        {
            Debug.LogError("No se encontró el componente Renderer en el paddle!");
        }

        ResetPaddleState();
    }

    private void Update()
    {
        HandleMovement();
        UpdatePowerUpState();
    }

    private void HandleMovement()
    {
        // Obtener input con soporte para teclado y touch
        float moveInput = Input.GetAxis("Horizontal");
        
        // Soporte para input táctil
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float screenHalfWidth = Screen.width * 0.5f;
            moveInput = (touch.position.x > screenHalfWidth) ? 1f : -1f;
        }

        // Movimiento directo sin smoothing
        float move = moveInput * speed * Time.deltaTime;
        Vector3 newPosition = transform.position + new Vector3(move, 0, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, -maxX, maxX);
        transform.position = newPosition;
    }

    public void ApplyPowerUp(float sizeMultiplier, Color newColor, float duration = 0f)
    {
        if (duration <= 0) duration = powerUpDuration;

        transform.localScale = new Vector3(
            defaultSize * sizeMultiplier, 
            transform.localScale.y, 
            transform.localScale.z
        );
        
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
        transform.localScale = new Vector3(defaultSize, transform.localScale.y, transform.localScale.z);
        paddleRenderer.material.color = defaultColor;
        isPoweredUp = false;
        currentPowerUpTime = 0;
        
        OnPowerUpExpired?.Invoke();
    }

    public event Action<float, Color, float> OnPowerUpApplied;
    public event Action OnPowerUpExpired;

    public void ResetPaddle()
    {
        transform.position = Vector3.zero;
        ResetPaddleState();
    }
}