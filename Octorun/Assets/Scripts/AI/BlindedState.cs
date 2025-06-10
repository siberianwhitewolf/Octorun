using UnityEngine;

public class BlindedState : IState
{
    private ChefAIController chef;
    private float blindDuration;
    private float timer;

    public BlindedState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        Debug.Log("Enter blinded state");
        timer = 0f;
        blindDuration = chef.blindTime;
        chef.inked.IsInked = true;
        chef.animator.SetBool("IsWalking",false);
        chef.animator.SetBool("IsRunning",false);
        chef.animator.SetTrigger("blind"); // Quizás caminar es más apropiado que correr
        chef.StopMovement();
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer > blindDuration)
        {
            if (chef.lineOfSight.CanSeeTarget)
            {
                chef.SwitchState(chef.chasingState);
            }
            chef.SwitchState(chef.patrollingState);
            
        }
    }

    public void Exit()
    {
        chef.isBlinded = false;
        chef.inked.IsInked = false;
    }

 
}