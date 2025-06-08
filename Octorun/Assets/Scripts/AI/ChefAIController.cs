using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent), typeof(LineOfSight), typeof(MaterialFloatLerpByInked))]
public class ChefAIController : MonoBehaviour, IBlindable
{
    [Header("References")] public Transform player;
    public Transform[] waypoints;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public Animator animator;

    [Header("Detection")] public float viewRadius = 10f;
    public float viewAngle = 45f;
    public float attackRange = 2f;
    public float blindTime = 5f;
    public bool isBlinded = false;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public int currentWaypointIndex = 0;

    private IState currentState;
    public LineOfSight lineOfSight;
    public MaterialFloatLerpByInked inked;

    // Estados disponibles
    public IdleState idleState;
    public CookingState cookingState;
    public PatrollingState patrollingState;
    public ChasingState chasingState;
    public AttackingState attackingState;
    public BlindedState blindedState;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        lineOfSight = GetComponent<LineOfSight>();
        inked = GetComponent<MaterialFloatLerpByInked>();

        // Crear instancias de los estados
        idleState = new IdleState(this);
        cookingState = new CookingState(this);
        patrollingState = new PatrollingState(this);
        chasingState = new ChasingState(this);
        attackingState = new AttackingState(this);
        blindedState = new BlindedState(this);
    }

    void Start()
    {
        SwitchState(idleState);
    }

    void Update()
    {
        currentState?.Update();

        if (lineOfSight != null && lineOfSight.CanSeeTarget && currentState != chasingState && !isBlinded)
        {
            SwitchState(chasingState);
        }

    }

    public void SwitchState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    
    public void Blind(float duration)
    {
        Debug.Log("Blinded for " + duration + " seconds");
        blindTime = duration;
        isBlinded = true;
        SwitchState(blindedState);
    }
    
public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer < viewRadius)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, obstacleMask))
                    return true;
            }
        }

        return false;
    }
    void OnDrawGizmosSelected()
    {
        // Dibuja FOV
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRadius);
    }
}
