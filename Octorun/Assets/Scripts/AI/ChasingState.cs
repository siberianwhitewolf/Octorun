using UnityEngine;

public class ChasingState : IState
{
    private ChefAIController chef;
    
    // Usaremos un cooldown para no solicitar un camino nuevo en cada fotograma,
    // lo que mejora enormemente el rendimiento.
    private float pathUpdateCooldown = 0.5f; 
    private float timer;

    public ChasingState(ChefAIController chef)
    {
        this.chef = chef;
    }

    public void Enter()
    {
        Debug.Log("Enter chase state");
        // Al entrar en el estado, activamos la animación de correr.
        chef.animator.SetBool("IsWalking", false);
        chef.animator.SetBool("IsRunning", true);
        chef.moveSpeed *= 2;
        
        // Forzamos una actualización del camino inmediatamente al entrar en el estado.
        timer = pathUpdateCooldown; 
    }

    public void Update()
    {
        // Si por alguna razón el jugador desaparece, no hacemos nada.
        if (chef.player == null)
        {
            chef.SwitchState(chef.patrollingState); // Volvemos a patrullar
            return;
        }

        timer += Time.deltaTime;

        // Actualizamos el camino hacia el jugador periódicamente en lugar de en cada frame.
        if (timer >= pathUpdateCooldown)
        {
            // --- LÓGICA DE MOVIMIENTO MODIFICADA ---
            // ANTES: chef.agent.SetDestination(chef.player.position);
            // AHORA: Le damos el destino al piloto. Él decidirá si ir directo o usar A*.
            chef.SetMovementTarget(chef.player.position);
            timer = 0f;
        }

        // La lógica para decidir si atacar o dejar de perseguir se mantiene igual.
        float distance = Vector3.Distance(chef.transform.position, chef.player.position);

        // Si estamos suficientemente cerca, atacamos.
        if (distance < chef.attackRange)
        {
            chef.SwitchState(chef.attackingState);
        }
        // Si perdemos la línea de visión, volvemos a patrullar.
        else if (chef.lineOfSight != null && !chef.lineOfSight.CanSeeTarget)
        {
            chef.SwitchState(chef.patrollingState);
        }
    }

    public void Exit()
    {
        // Al salir del estado de persecución, detenemos la animación de correr.
        chef.animator.SetBool("IsRunning", false);
        chef.moveSpeed /= 2;
        
        // Opcional pero recomendado: Detener el movimiento para que no se siga deslizando.
        chef.StopMovement();
    }
}