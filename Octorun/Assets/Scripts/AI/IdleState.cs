using UnityEngine;

public class IdleState : IState
{
    private ChefAIController chef;
    private float timer;
    private float idleDuration = 3f;

    public IdleState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        timer = 0f;
        chef.StopMovement(); 
        chef.animator.SetBool("IsWalking", false);
        chef.animator.SetBool("IsRunning", false);
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (chef.lineOfSight != null && chef.lineOfSight.CanSeeTarget)
        {
            chef.SwitchState(chef.chasingState);
            return;
        }

        if (timer >= idleDuration)
        {
            chef.SwitchState(chef.patrollingState);
        }
    }

    public void Exit() { }
}