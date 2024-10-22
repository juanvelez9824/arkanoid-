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
            GameManager.Instance.lives++;
            UIManager.Instance.UpdateLivesUI(GameManager.Instance.lives);
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.powerUp);
    }
}
