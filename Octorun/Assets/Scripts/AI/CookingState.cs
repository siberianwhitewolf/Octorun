using UnityEngine;

public class CookingState : IState
{
    private ChefAIController chef;

    public CookingState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        chef.agent.SetDestination(chef.transform.position);
    }

    public void Update()
    {
        if (chef.lineOfSight != null && chef.lineOfSight.CanSeeTarget)
            chef.SwitchState(chef.chasingState);
    }

    public void Exit() { }
}