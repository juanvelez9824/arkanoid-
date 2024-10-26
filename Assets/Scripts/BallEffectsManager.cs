using UnityEngine;

public class BallEffectsManager : MonoBehaviour 
{


    [Header("Referencias de Sistemas de Partículas")]
    [SerializeField] public ParticleSystem collisionEffect;
    [SerializeField] public ParticleSystem trailEffect;
    [SerializeField] public ParticleSystem powerUpEffect;

    [Header("Configuración de Colores Retro")]
    [SerializeField] private Color neonPink = new Color(1f, 0f, 0.75f, 1f);
    [SerializeField] private Color neonBlue = new Color(0f, 0.75f, 1f, 1f);
    [SerializeField] private Color neonPurple = new Color(0.75f, 0f, 1f, 1f);
    [SerializeField] private Color neonYellow = new Color(1f, 0.9f, 0f, 1f);

    [Header("Configuración de Efectos Retro")]
    [SerializeField] private Material pixelParticleMaterial; // Asignar material con shader para partículas pixeladas
    [SerializeField] private float gridSnap = 0.125f; // Para efecto pixelado en movimiento

    private void Start()
    {
        SetupCollisionEffect();
        SetupTrailEffect();
        SetupPowerUpEffect();
    }

    private void SetupCollisionEffect()
    {
        if (collisionEffect != null)
        {
            var main = collisionEffect.main;
            main.playOnAwake = false;
            main.loop = false;
            main.duration = 0.3f;
            main.startLifetime = 0.8f;
            main.startSpeed = 3f;
            main.startSize = 0.3f;
            main.startRotation = 0f; // Mantener rotación fija para formas geométricas
            
            // Configurar para formas geométricas
            var shape = collisionEffect.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(0.2f, 0.2f, 0.1f);

            // Emisión en patrón geométrico
            var emission = collisionEffect.emission;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 8, 8, 0.1f)
            });

            // Agregar rotación controlada
            var rotationOverLifetime = collisionEffect.rotationOverLifetime;
            rotationOverLifetime.enabled = true;
            rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(45f);

            // Configurar el renderer para efecto pixelado
            var renderer = collisionEffect.GetComponent<ParticleSystemRenderer>();
            renderer.material = pixelParticleMaterial;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sortMode = ParticleSystemSortMode.None;
        }
    }

    private void SetupTrailEffect()
    {
        if (trailEffect != null)
        {
            var main = trailEffect.main;
            main.playOnAwake = true;
            main.loop = true;
            main.startLifetime = 0.4f;
            main.startSpeed = 0f;
            main.startSize = 0.2f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = trailEffect.emission;
            emission.rateOverTime = 15;

            // Agregar efecto de destello
            var colorOverLifetime = trailEffect.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(neonBlue, 0.0f),
                    new GradientColorKey(neonPurple, 0.5f),
                    new GradientColorKey(neonBlue, 1.0f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0.0f, 1.0f)
                }
            );
            colorOverLifetime.color = gradient;

            // Configurar el renderer
            var renderer = trailEffect.GetComponent<ParticleSystemRenderer>();
            renderer.material = pixelParticleMaterial;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }
    }

    private void SetupPowerUpEffect()
    {
        if (powerUpEffect != null)
        {
            var main = powerUpEffect.main;
            main.playOnAwake = false;
            main.loop = true;
            main.startLifetime = 0.7f;
            main.startSpeed = 2.5f;
            main.startSize = 0.3f;
            
            // Configurar forma geométrica
            var shape = powerUpEffect.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(0.4f, 0.4f, 0.4f);

            var emission = powerUpEffect.emission;
            emission.rateOverTime = 20;

            // Agregar movimiento oscilante
            var velocityOverLifetime = powerUpEffect.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(0f, new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.5f, 1),
                new Keyframe(1, 0)
            ));

            // Configurar el renderer
            var renderer = powerUpEffect.GetComponent<ParticleSystemRenderer>();
            renderer.material = pixelParticleMaterial;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }
    }

    private void Update()
    {
        // Aplicar efecto de movimiento pixelado
        if (trailEffect != null)
        {
            Vector3 position = trailEffect.transform.position;
            position.x = Mathf.Round(position.x / gridSnap) * gridSnap;
            position.y = Mathf.Round(position.y / gridSnap) * gridSnap;
            trailEffect.transform.position = position;
        }
    }

    public void PlayCollisionEffect(Vector3 position, CollisionType type)
    {
        if (collisionEffect != null)
        {
            // Redondear posición para efecto pixelado
            position.x = Mathf.Round(position.x / gridSnap) * gridSnap;
            position.y = Mathf.Round(position.y / gridSnap) * gridSnap;
            collisionEffect.transform.position = position;
            
            var main = collisionEffect.main;
            switch (type)
            {
                case CollisionType.Paddle:
                    main.startColor = neonPurple;
                    break;
                case CollisionType.Wall:
                    main.startColor = neonBlue;
                    break;
                case CollisionType.Block:
                    main.startColor = neonPink;
                    break;
            }
            
            collisionEffect.Play();
        }
    }

    public void StartPowerUpEffect(Color color)
    {
        if (powerUpEffect != null)
        {
            var main = powerUpEffect.main;
            main.startColor = neonYellow; // Usar color neón para power-ups
            powerUpEffect.Play();
        }
    }

    public void StopPowerUpEffect()
    {
        if (powerUpEffect != null)
        {
            powerUpEffect.Stop();
        }
    }
}

public enum CollisionType
{
    Paddle,
    Wall,
    Block,
    TopBoundary
}
