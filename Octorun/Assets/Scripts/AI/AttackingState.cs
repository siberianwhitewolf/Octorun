using UnityEngine;

public class AttackingState : IState
{
    private ChefAIController chef;
    private float attackCooldown = 2f;
    private float timer;

    public AttackingState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        chef.agent.SetDestination(chef.transform.position);
        timer = 0f;
    }

    public void Update()
    {
        if (chef.player == null) return;

        timer += Time.deltaTime;
        chef.transform.LookAt(chef.player);

        if (timer >= attackCooldown)
        {
            // Aquí podrías aplicar daño o animación
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