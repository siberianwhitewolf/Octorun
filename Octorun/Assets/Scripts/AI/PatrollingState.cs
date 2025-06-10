using UnityEngine;

public class PatrollingState : IState
{
    private ChefAIController chef;
    
    // Asumimos que tienes un script llamado WaypointNode en tus objetos de waypoint
    // con variables públicas como 'waitTime' y 'triggerCookingHere'.
    private WaypointNode currentNode; 
    
    private bool isWaiting;
    private float waitTimer;
    private int direction = 1; // +1 para adelante, -1 para atrás en la ruta de patrulla

    public PatrollingState(ChefAIController chef)
    {
        this.chef = chef;
    }

    public void Enter()
    {
        Debug.Log("Enter patrolling state");
        isWaiting = false;
        waitTimer = 0f;
        
        // Al entrar en el estado, nos aseguramos de que empiece a caminar.
        chef.animator.SetBool("IsWalking", true);
        chef.animator.SetBool("IsRunning", false);

        // Le decimos que se mueva a su waypoint actual al iniciar la patrulla.
        MoveToWaypoint(chef.currentWaypointIndexPatrol);
    }

    public void Update()
    {
        // La detección del jugador siempre tiene la máxima prioridad.
        if (chef.lineOfSight != null && chef.lineOfSight.CanSeeTarget)
        {
            chef.SwitchState(chef.chasingState);
            return;
        }

        // Si está en modo de espera en un waypoint...
        if (isWaiting)
        {
            // Detenemos la animación de caminar mientras espera.
            chef.animator.SetBool("IsWalking", false);
            waitTimer += Time.deltaTime;
            if (waitTimer >= currentNode.waitTime)
            {
                isWaiting = false;
                AdvanceIndex();
                MoveToWaypoint(chef.currentWaypointIndexPatrol);
            }
            return;
        }

        // --- LÓGICA DE LLEGADA MODIFICADA ---
        // Usamos la nueva propiedad del ChefAIController, que es mucho más simple y
        // funciona tanto para movimiento directo como para pathfinding A*.
        if (chef.HasReachedDestination)
        {
            // Hemos llegado al waypoint. Decidimos qué hacer.
            if (currentNode != null)
            {
                // ¿Debemos cambiar al estado de cocinar?
                if (currentNode.triggerCookingHere)
                {
                    chef.SwitchState(chef.cookingState);
                    return;
                }

                // ¿Debemos esperar aquí un tiempo?
                if (currentNode.waitTime > 0f)
                {
                    isWaiting = true;
                    waitTimer = 0f;
                    return;
                }
            }

            // Si no hay que cocinar ni esperar, avanzamos al siguiente waypoint inmediatamente.
            AdvanceIndex();
            MoveToWaypoint(chef.currentWaypointIndexPatrol);
        }
    }

    public void Exit()
    {
        // Nos aseguramos de limpiar el estado de espera y las animaciones al salir.
        isWaiting = false;
        chef.animator.SetBool("IsWalking", false);
    }

    private void AdvanceIndex()
    {
        if (chef.waypoints.Length <= 1) return; // Evitar errores si no hay suficientes waypoints

        // Lógica de patrulla en "ping-pong" para recorrer la lista de waypoints.
        if (chef.currentWaypointIndexPatrol <= 0)
        {
            direction = 1; // Si llega al principio, avanza.
        }
        else if (chef.currentWaypointIndexPatrol >= chef.waypoints.Length - 1)
        {
            direction = -1; // Si llega al final, retrocede.
        }

        chef.currentWaypointIndexPatrol += direction;
    }

    private void MoveToWaypoint(int index)
    {
        if (chef.waypoints.Length == 0 || index < 0 || index >= chef.waypoints.Length) return;

        Transform waypointTransform = chef.waypoints[index];
        currentNode = waypointTransform.GetComponent<WaypointNode>();

        // --- LÓGICA DE MOVIMIENTO MODIFICADA ---
        // En lugar de controlar un NavMeshAgent, simplemente le damos el destino a nuestro "piloto"
        // en ChefAIController. Él se encargará de decidir cómo llegar.
        chef.SetMovementTarget(waypointTransform.position);
        
        // Nos aseguramos de que la animación de caminar esté activa.
        chef.animator.SetBool("IsWalking", true);
    }
}