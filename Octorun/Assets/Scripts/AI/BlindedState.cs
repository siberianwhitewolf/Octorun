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
        chef.inked.IsInked = true;
        chef.animator.SetBool("IsWalking", true); // Quizás caminar es más apropiado que correr
        WanderRandomly();
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= blindDuration)
        {
            if (chef.lineOfSight != null && chef.lineOfSight.CanSeeTarget)
                chef.SwitchState(chef.chasingState);
            else
                chef.SwitchState(chef.patrollingState);
            return;
        }

        // Si ha llegado a su destino aleatorio, busca otro.
        if (chef.HasReachedDestination)
        {
            WanderRandomly();
        }
    }

    public void Exit()
    {
        chef.isBlinded = false;
        chef.inked.IsInked = false;
        chef.animator.SetBool("IsWalking", false);
    }

    private void WanderRandomly()
    {
        Vector3 randomDir = Random.insideUnitSphere * 5f; // Radio de 5m
        randomDir += chef.transform.position;
        
        // Buscamos un nodo caminable cerca de ese punto aleatorio
        Node targetNode = GridManager.Instance.Pathfinder.GetNearestWalkableNode(randomDir);
        if (targetNode != null)
        {
            chef.SetMovementTarget(targetNode.transform.position);
        }
    }
}