using UnityEngine;

public class OctopusInk : MonoBehaviour
{
    [Header("Coste y Efecto")]
    [Tooltip("Cuánta tinta consume la habilidad por segundo.")]
    public float inkCostPerSecond = 10f;
    [Tooltip("Fuerza del retroceso al disparar.")]
    public float recoilForce = 3f;
    public KeyCode fireKey = KeyCode.Mouse0;
    
    [Header("Spray Settings")]
    public float sprayRadius = 4f;
    public float sprayAngle = 30f;
    public float blindDuration = 5f;
    public float blindInterval = 1f;
    public LayerMask enemyMask;

    [Header("Referencias")]
    public ParticleSystem inkParticle;
    public Entity playerEntity;

    // --- Variables eliminadas: inkMax, currentInk, inkRechargeRate ---
    // --- Nueva referencia al InkManager ---
    private InkManager inkManager;

    private ParticleSystem[] allInkParticles;
    private bool isFiring = false;
    private Rigidbody rb;
    private float blindTimer;
    
    void Awake() // Cambiado de Start a Awake para asegurar que la referencia exista antes que otros Start() la necesiten
    {
        inkManager = GetComponent<InkManager>();
        if (inkManager == null)
        {
            Debug.LogError("El componente InkManager no se encuentra en el jugador. El script OctopusInk no funcionará.", this);
            enabled = false;
            return;
        }

        allInkParticles = inkParticle.GetComponentsInChildren<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        blindTimer -= Time.deltaTime;
        bool shootPressed = Input.GetKey(fireKey);

        if (shootPressed && playerEntity.IsAlive)
        {
            // --- LÓGICA DE CONSUMO MODIFICADA ---
            // Intentamos usar la tinta. Si InkManager nos da permiso (devuelve true)...
            if (inkManager.UseInk(inkCostPerSecond * Time.deltaTime))
            {
                // ...entonces activamos los efectos.
                if (!isFiring)
                {
                    StartFiringEffects();
                }
                ApplyRecoil();
                TryBlindTargets();
            }
            else // Si UseInk devuelve false (no hay tinta)...
            {
                // ...detenemos los efectos.
                StopFiringEffects();
            }
        }
        else
        {
            // Si dejamos de presionar la tecla, detenemos los efectos.
            StopFiringEffects();
        }
        // --- La lógica de regeneración ha sido eliminada de aquí. El InkManager se encarga. ---
    }

    void StartFiringEffects()
    {
        if (isFiring) return;
        foreach (var ps in allInkParticles)
            ps.Play();
        isFiring = true;
    }

    void StopFiringEffects()
    {
        if (!isFiring) return;
        foreach (var ps in allInkParticles)
            ps.Stop();
        isFiring = false;
    }

    void ApplyRecoil()
    {
        if (rb == null) return;
        Vector3 recoilDir = transform.up;
        rb.AddForce(recoilDir * recoilForce, ForceMode.Impulse);
    }

    void TryBlindTargets()
    {
        if (blindTimer > 0f) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, sprayRadius, enemyMask);
        Vector3 sprayDirection = -transform.up;

        foreach (Collider hit in hits)
        {
            IBlindable blindable = hit.GetComponentInParent<IBlindable>();
            if (blindable != null)
            {
                Vector3 toTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(sprayDirection, toTarget);

                Debug.DrawLine(transform.position, hit.transform.position, Color.green, 1f);

                if (angle <= sprayAngle)
                {
                    blindable.Blind(blindDuration);
                    Debug.Log("Cegado: " + hit.name);
                }
            }
        }

        blindTimer = blindInterval;
    }

    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.9f); // Rojo brillante con alta opacidad
        
        Vector3 origin = transform.position;
        Vector3 forward = -transform.up; // Dirección del spray

        int segments = 30;
        float angleStep = (sprayAngle * 2f) / segments;
        Vector3 previousPoint = origin + Quaternion.Euler(0, -sprayAngle, 0) * forward * sprayRadius;

        // Dibuja líneas radiales desde el origen
        for (int i = 1; i <= segments; i++)
        {
            float angle = -sprayAngle + (angleStep * i);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * forward;
            Vector3 point = origin + dir.normalized * sprayRadius;

            Gizmos.DrawLine(origin, point);         // línea radial
            Gizmos.DrawLine(previousPoint, point);  // línea de arco

            previousPoint = point;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sprayRadius);
    }
}
