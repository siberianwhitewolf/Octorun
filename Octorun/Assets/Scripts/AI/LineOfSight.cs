using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public Transform target;
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public bool CanSeeTarget { get; private set; }

    void Update()
    {
        CanSeeTarget = CheckLineOfSight();
    }

    private bool CheckLineOfSight()
    {
        if (target == null) return false;

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget <= viewRadius)
        {
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            if (angle < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, target.position, obstacleMask))
                    return true;
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, viewRadius);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.color = CanSeeTarget ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + left * viewRadius);
        Gizmos.DrawLine(origin, origin + right * viewRadius);

        if (target != null && CanSeeTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, target.position);
        }
    }
}