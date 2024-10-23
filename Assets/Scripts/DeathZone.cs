using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Optional Settings")]
    [SerializeField] private bool showDebugVisuals = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private Vector3 triggerSize = new Vector3(30f, 1f, 10f); 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Bola detectada en la zona de muerte");
            
            // Reducir vida en el GameManager
            GameManager.Instance?.LoseLife();
            
            // Obtener referencia al BallController
            BallController ball = other.GetComponent<BallController>();
            if (ball != null)
            {
                // Usar el método existente para adherir la bola al paddle
                ball.AttachBallToPaddle();
            }
        }
    }

    // Visualización en el editor para facilitar el posicionamiento
    private void OnDrawGizmos()
    {
        if (!showDebugVisuals) return;

        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, triggerSize);
    }
}
