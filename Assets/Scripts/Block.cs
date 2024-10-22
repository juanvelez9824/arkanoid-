using UnityEngine;
public class Block : MonoBehaviour
{
    public int hitPoints = 2;  // Número de golpes para destruir
    private Renderer renderer;
    public Color damagedColor;  // Color del bloque después de recibir un golpe

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitPoints--;
        
        if (hitPoints > 0)
        {
            renderer.material.color = damagedColor;  // Cambiar el color cuando está dañado
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.blockDestroy);
            Destroy(gameObject);
            GameManager.Instance.AddScore(10);
        }
    }
}
