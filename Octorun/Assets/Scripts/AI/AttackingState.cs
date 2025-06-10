using UnityEngine;

public class AttackingState : IState
{
    private ChefAIController chef;
    // El cooldown del ataque se puede mantener, es la frecuencia de los golpes.
    private float attackCooldown = 2f; 
    private float timer;

    public AttackingState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        Debug.Log("Enter attacking state");
        chef.StopMovement(); // El Chef se detiene para atacar.
        timer = 0f; // Hacemos que pueda atacar inmediatamente al entrar en el estado.
        chef.animator.SetBool("IsRunning", false);
        chef.animator.SetBool("IsWalking", false);
    }

    public void Update()
    {
        // Siempre rotar para mirar al jugador mientras está en rango de ataque.
        Vector3 direction = (chef.player.position - chef.transform.position).normalized;
        if(direction != Vector3.zero)
            chef.transform.rotation = Quaternion.LookRotation(direction);
        
        // Lógica de ataque con cooldown
        timer += Time.deltaTime;
        if (timer > attackCooldown)
        {
            chef.animator.SetTrigger("Attack");
            // Aquí aplicarías el daño al jugador.
            chef.playerEntity.TakeDamage(chef.damage); // Necesitarías una referencia a la entidad del jugador.
            Debug.Log("Chef ataca!");
            timer = 0f;
        }

        // --- CONDICIÓN DE SALIDA CORREGIDA ---
        float dist = Vector3.Distance(chef.transform.position, chef.player.position);

        // Volvemos a perseguir si el jugador se aleja DEMASIADO (más allá de la nueva distancia)
        // O si perdemos la línea de visión directa con él (se esconde detrás de una columna).
        Debug.Log("distance: "+ dist);
        Debug.Log("disengage: "+ chef.disengageDistance);
        if (dist > chef.disengageDistance || !chef.lineOfSight.CanSeeTarget)
        {
            chef.SwitchState(chef.chasingState);
        }
    }

    public void Exit()
    {
        // Limpieza al salir del estado. No es estrictamente necesario aquí.
    }
}