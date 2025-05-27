using UnityEngine;

public class ChasingState : IState
{
    private ChefAIController chef;

    public ChasingState(ChefAIController chef) => this.chef = chef;

    public void Enter() { }

    public void Update()
    {
        if (chef.player == null) return;

        chef.agent.SetDestination(chef.player.position);

        float distance = Vector3.Distance(chef.transform.position, chef.player.position);

        if (distance <= chef.attackRange)
        {
            chef.SwitchState(chef.attackingState);
        }
        else if (chef.lineOfSight != null && !chef.lineOfSight.CanSeeTarget)
        {
            chef.SwitchState(chef.patrollingState);
        }
    }

    public void Exit() { }
}