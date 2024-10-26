using UnityEngine;

public class BlockFloatingEffect : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatHeight = 0.1f;
    [SerializeField] private float rotationSpeed = 30f;
    
    [Header("Random Offset")]
    [SerializeField] private bool useRandomOffset = true;
    
    private Vector3 startPosition;
    private float randomOffset;
    
    void Start()
    {
        startPosition = transform.position;
        if (useRandomOffset)
        {
            randomOffset = Random.Range(0f, 2f * Mathf.PI);
        }
    }
    
    void Update()
    {
        // Calcular movimiento flotante
        float yOffset = Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatHeight;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
        
        // Añadir rotación suave
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
