using UnityEngine;

public class PowerUp : MonoBehaviour 
{
    public enum PowerUpType { EnlargePaddle, ExtraLife }
    public PowerUpType type;
    public Color powerUpColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paddle"))
        {
            ApplyPowerUp(other.gameObject);
            Destroy(gameObject);
        }
    }

    void ApplyPowerUp(GameObject paddle)
    {
        PaddleController paddleController = paddle.GetComponent<PaddleController>();

        if (type == PowerUpType.EnlargePaddle)
        {
            paddleController.ApplyPowerUp(1.5f, powerUpColor);  // Agranda y cambia el color del paddle
        }
        else if (type == PowerUpType.ExtraLife)
        {
            // En lugar de intentar modificar lives directamente,
            // deberíamos agregar un método en GameManager para añadir vidas
            int currentLives = GameManager.Instance.GetCurrentLives();
            // Sumar una vida y actualizar la UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateLivesUI(currentLives + 1);
            }
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.powerUp);
    }
}
