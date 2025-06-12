using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public Transform target;
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public Transform eyeLevel;
    
    // --- NUEVA VARIABLE ---
    [Tooltip("La altura desde el pivote del Chef desde donde se origina la visión (sus 'ojos').")]
    public float eyeHeight = 1.5f;

    public LayerMask targetMask; // Nota: Esta variable no se está usando, pero la dejo por si la necesitas.
    public LayerMask obstacleMask;
    public OctopusController octopus; // Asumo que el pulpo es el jugador/objetivo.

    public bool CanSeeTarget { get; private set; }

    void Update()
    {
        CanSeeTarget = CheckLineOfSight();
        Debug.Log(CanSeeTarget);
        
        // --- NUEVO: DIBUJAR LÍNEA DE DEBUG EN TIEMPO REAL ---
        // Esto te permitirá ver exactamente qué está pasando en la vista de Scene.
        #if UNITY_EDITOR
        if (target != null)
        {
            Vector3 origin = transform.position + Vector3.up * eyeHeight;
            Vector3 targetCenter = target.position + Vector3.up * (eyeHeight / 2); // Apuntamos al centro del torso del objetivo
            Color debugColor = CanSeeTarget ? Color.green : Color.red;
            Debug.DrawLine(origin, targetCenter, debugColor);
        }
        #endif
    }

    private bool CheckLineOfSight()
    {
        // La comprobación del estado 'hiding' del pulpo es correcta.
        if (target == null || (octopus != null && octopus.isHiding) || !octopus.isAlive) return false;

        // --- LÓGICA DE VISIÓN MODIFICADA ---
        
        // 1. Calculamos la distancia al objetivo.
        float distToTarget = Vector3.Distance(transform.position, target.position);
        
        // Si el objetivo está fuera del radio de visión, no lo vemos.
        if (distToTarget > viewRadius)
        {
            return false;
        }

        // 2. Comprobamos el ángulo de visión.
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToTarget);

        // Si el objetivo está fuera del cono de visión, no lo vemos.
        if (angle >= viewAngle / 2f)
        {
            return false;
        }

        // 3. --- COMPROBACIÓN DE OBSTÁCULOS CORREGIDA ---
        // Lanzamos el rayo desde la altura de los ojos hacia el centro del torso del objetivo.
        Vector3 origin = eyeLevel.position;
        Vector3 targetCenter = target.position; 

        // Si el Linecast NO golpea ningún obstáculo, entonces SÍ podemos ver al objetivo.
        if (Physics.Linecast(origin, targetCenter, targetMask))
        {
            return true;
        }

        // Si alguna de las comprobaciones anteriores falló, no vemos al objetivo.
        return false;
    }

    void OnDrawGizmosSelected()
    {
        // Tus gizmos para el radio y ángulo de visión están perfectos, los mantenemos.
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, viewRadius);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.color = Color.blue; // Lo cambio a azul para que no se confunda con la línea de debug
        Gizmos.DrawLine(origin, origin + left * viewRadius);
        Gizmos.DrawLine(origin, origin + right * viewRadius);
    }
}