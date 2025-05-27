using UnityEngine;

public class BlindedState : IState
{
    private ChefAIController chef;
    private float blindDuration;
    private float timer;

    public BlindedState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        timer = 0f;
        blindDuration = chef.blindTime;

        // Movimiento errático
        WanderRandomly();
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= blindDuration)
        {
            if (chef.lineOfSight != null && chef.lineOfSight.CanSeeTarget)
            {
                chef.SwitchState(chef.chasingState);
            }
            else
            {
                chef.SwitchState(chef.patrollingState);
            }
            return;
        }

        if (!chef.agent.pathPending && chef.agent.remainingDistance <= chef.agent.stoppingDistance)
        {
            WanderRandomly();
        }
    }

    public void Exit()
    {
        chef.isBlinded = false;
    }

    private void WanderRandomly()
    {
        Vector3 randomDir = Random.insideUnitSphere;
        randomDir.y = 0;
        Vector3 destination = chef.transform.position + randomDir.normalized * Random.Range(2f, 5f);

        if (UnityEngine.AI.NavMesh.SamplePosition(destination, out UnityEngine.AI.NavMeshHit hit, 3f, UnityEngine.AI.NavMesh.AllAreas))
        {
            chef.agent.SetDestination(hit.position);
        }
    }
}