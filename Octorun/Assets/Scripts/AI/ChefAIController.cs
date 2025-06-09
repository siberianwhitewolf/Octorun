using System.Collections.Generic;
using UnityEngine;

// Requerimos un Rigidbody, ya que ahora es el motor del movimiento.
[RequireComponent(typeof(Rigidbody), typeof(LineOfSight), typeof(MaterialFloatLerpByInked))]
public class ChefAIController : MonoBehaviour, IBlindable
{
    // --- Modos de Movimiento ---
    private enum MovementMode { None, Direct, Path }
    private MovementMode currentMode = MovementMode.None;

    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 10f;

    [Header("References")]
    public Transform player;
    public Transform[] waypoints;
    public LayerMask playerMask;
    [Tooltip("La capa de los obstáculos que bloquean la visión directa (paredes, columnas, etc.).")]
    public LayerMask obstacleMask; // MUY IMPORTANTE
    public Animator animator;

    [Header("Detection")]
    public float viewRadius = 10f;
    public float viewAngle = 45f;
    public float attackRange = 2f;
    public float blindTime = 5f;
    public bool isBlinded = false;

    // --- Variables de Pathfinding y Movimiento ---
    private List<Node> path;
    private int currentWaypointIndex;
    private Vector3 directMovementTarget;
    private Rigidbody rb;

    // --- Variables de Estados y Componentes ---
    [HideInInspector] public int currentWaypointIndexPatrol = 0;
    private IState currentState;
    public LineOfSight lineOfSight;
    public MaterialFloatLerpByInked inked;

    // Estados
    public IdleState idleState;
    public CookingState cookingState;
    public PatrollingState patrollingState;
    public ChasingState chasingState;
    public AttackingState attackingState;
    public BlindedState blindedState;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Congelamos la rotación para que no se caiga

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
        
        // La lógica de ver al jugador para cambiar a ChasingState se mantiene
        if (lineOfSight != null && lineOfSight.CanSeeTarget && currentState != chasingState && !isBlinded)
        {
            SwitchState(chasingState);
        }
    }
    
    void FixedUpdate()
    {
        // El movimiento físico ahora se gestiona aquí, en FixedUpdate, para ser compatible con la física.
        ExecuteMovement();
    }

    // --- FUNCIONES DEL NUEVO PILOTO HÍBRIDO ---

    /// <summary>
    /// La función principal que los estados usarán. Decide si moverse directo o usar A*.
    /// </summary>
    public void SetMovementTarget(Vector3 destination)
    {
        if (HasLineOfSightTo(destination))
        {
            // Si hay visión directa, nos movemos en línea recta.
            currentMode = MovementMode.Direct;
            directMovementTarget = destination;
            path = null; // Limpiamos cualquier camino A* anterior
        }
        else
        {
            // Si no, solicitamos un camino al sistema A*.
            currentMode = MovementMode.Path;
            path = GridManager.Instance.Pathfinder.FindPath(transform.position, destination);
            currentWaypointIndex = 0;
        }
    }

    /// <summary>
    /// Detiene completamente cualquier tipo de movimiento.
    /// </summary>
    public void StopMovement()
    {
        currentMode = MovementMode.None;
        rb.velocity = Vector3.zero;
        path = null;
    }

    /// <summary>
    /// Propiedad pública para que los estados sepan si el agente ha llegado a su destino actual.
    /// </summary>
    public bool HasReachedDestination
    {
        get
        {
            if (currentMode == MovementMode.None) return true;
            if (currentMode == MovementMode.Direct)
            {
                // Si estamos cerca del objetivo directo, hemos llegado.
                return Vector3.Distance(transform.position, directMovementTarget) < 0.5f;
            }
            if (currentMode == MovementMode.Path)
            {
                // Si el camino no existe o ya lo hemos recorrido, hemos llegado.
                return path == null || currentWaypointIndex >= path.Count;
            }
            return true;
        }
    }

    /// <summary>
    /// Comprueba si hay una línea de visión directa a un punto, sin obstáculos.
    /// </summary>
    private bool HasLineOfSightTo(Vector3 target)
    {
        // Usamos un Linecast, que es como un Raycast pero entre dos puntos.
        // Si golpea algo en la capa de obstáculos, no hay línea de visión.
        // Añadimos un poco de altura para evitar que el rayo choque con el suelo.
        return !Physics.Linecast(transform.position + Vector3.up * 0.5f, target + Vector3.up * 0.5f, obstacleMask);
    }

    /// <summary>
    /// Mueve físicamente al personaje según el modo de movimiento actual.
    /// </summary>
    private void ExecuteMovement()
    {
        if (currentMode == MovementMode.None || HasReachedDestination)
        {
            // Si no hay que moverse o ya llegamos, detenemos las animaciones y salimos.
            rb.velocity = Vector3.zero; // Aseguramos que se detenga
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            return;
        }

        Vector3 direction = Vector3.zero;
        
        if (currentMode == MovementMode.Direct)
        {
            direction = (directMovementTarget - transform.position).normalized;
        }
        else if (currentMode == MovementMode.Path)
        {
            Vector3 currentWaypoint = path[currentWaypointIndex].transform.position;
            if (Vector3.Distance(transform.position, currentWaypoint) < 0.5f)
            {
                currentWaypointIndex++;
                // Si este era el último waypoint, HasReachedDestination se volverá true en el próximo frame.
                if (currentWaypointIndex >= path.Count) return;
            }
            direction = (path[currentWaypointIndex].transform.position - transform.position).normalized;
        }

        // Aplicamos movimiento y rotación
        direction.y = 0; // Nos aseguramos de que el movimiento sea en el plano XZ
        if (direction.sqrMagnitude > 0.01f)
        {
            rb.velocity = direction * moveSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);
        }
    }
    
    // --- El resto de tus funciones de siempre ---
    
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRadius);
    }
}