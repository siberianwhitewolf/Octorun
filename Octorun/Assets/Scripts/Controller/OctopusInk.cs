using System.Collections;
using UnityEngine;

public class OctopusInk : MonoBehaviour
{
    [Header("Ink Settings")]
    public float inkMax = 100f;
    public float inkCostPerSecond = 10f;
    public float inkRechargeRate = 5f;
    public float recoilForce = 3f;
    public KeyCode fireKey = KeyCode.Mouse0;
    public LayerMask enemyMask;


    [Header("Spray Settings")]
    public float sprayRadius = 4f;
    public float sprayAngle = 30f;
    public float blindDuration = 5f;
    public float blindInterval = 1f;

    public float currentInk;
    public ParticleSystem inkParticle;

    private ParticleSystem[] allInkParticles;
    private bool isFiring = false;
    private Rigidbody rb;
    public float blindTimer;

    void Start()
    {
        allInkParticles = inkParticle.GetComponentsInChildren<ParticleSystem>();
        currentInk = inkMax;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        blindTimer -= Time.deltaTime;
        bool shootPressed = Input.GetKey(fireKey);

        if (shootPressed && currentInk > 0f)
        {
            if (!isFiring)
            {
                foreach (var ps in allInkParticles)
                    ps.Play();

                isFiring = true;
            }

            ApplyRecoil();
            TryBlindTargets();

            currentInk -= inkCostPerSecond * Time.deltaTime;
            currentInk = Mathf.Max(0f, currentInk);

            if (currentInk == 0)
            {
                foreach (var ps in allInkParticles)
                    ps.Stop();
            }
        }
        else
        {
            if (isFiring)
            {
                foreach (var ps in allInkParticles)
                    ps.Stop();

                isFiring = false;
            }

            currentInk += inkRechargeRate * Time.deltaTime;
            currentInk = Mathf.Min(inkMax, currentInk);
        }
    }

    void ApplyRecoil()
    {
        if (rb == null) return;

        Vector3 recoilDir = transform.up; // hacia la espalda del pulpo
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



    public float GetInkLevelNormalized() => currentInk / inkMax;
}
