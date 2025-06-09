using UnityEngine;

public class AttackingState : IState
{
    private ChefAIController chef;
    private float attackCooldown = 2f;
    private float timer;

    public AttackingState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        chef.StopMovement(); // Le decimos al piloto que se detenga
        timer = 0f;
    }

    public void Update()
    {
        if (chef.player == null) return;

        timer += Time.deltaTime;
        // Rotar para mirar al jugador
        Vector3 direction = (chef.player.position - chef.transform.position).normalized;
        if(direction != Vector3.zero)
            chef.transform.rotation = Quaternion.LookRotation(direction);

        if (timer >= attackCooldown)
        {
            chef.animator.SetTrigger("Attack");
            // Aquí aplicarías daño, etc.
            timer = 0f;
        }

        float dist = Vector3.Distance(chef.transform.position, chef.player.position);
        if (dist > chef.attackRange)
        {
            chef.SwitchState(chef.chasingState);
        }
    }

    public void Exit() { }
}