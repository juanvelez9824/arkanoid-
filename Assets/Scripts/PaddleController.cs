using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f;
    public float maxX = 7.5f;
    private Renderer paddleRenderer;

    private void Start()
    {
        paddleRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector3 newPosition = transform.position + new Vector3(move, 0, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, -maxX, maxX);
        transform.position = newPosition;
    }

    public void ApplyPowerUp(float sizeMultiplier, Color newColor)
    {
        transform.localScale = new Vector3(sizeMultiplier, transform.localScale.y, transform.localScale.z);
        paddleRenderer.material.color = newColor;  // Cambia el color del paddle
        AudioManager.Instance.PlaySFX(AudioManager.Instance.powerUp);  // Reproduce el sonido del power-up
    }
}
