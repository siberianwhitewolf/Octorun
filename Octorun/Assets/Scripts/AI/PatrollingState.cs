using UnityEngine;

public class PatrollingState : IState
{
    private ChefAIController chef;
    private WaypointNode currentNode;
    private bool isWaiting;
    private float waitTimer;

    private int direction = 1; // +1: forward, -1: backward

    public PatrollingState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        isWaiting = false;
        waitTimer = 0f;
        MoveToWaypoint(chef.currentWaypointIndex); // No saltear el primero
    }

    public void Update()
    {
        if (chef.lineOfSight != null && chef.lineOfSight.CanSeeTarget)
        {
            chef.SwitchState(chef.chasingState);
            return;
        }

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= currentNode.waitTime)
            {
                isWaiting = false;
                AdvanceIndex();
                MoveToWaypoint(chef.currentWaypointIndex);
            }
            return;
        }

        if (!chef.agent.pathPending && chef.agent.remainingDistance <= chef.agent.stoppingDistance)
        {
            if (currentNode != null)
            {
                if (currentNode.triggerCookingHere)
                {
                    chef.SwitchState(chef.cookingState);
                    return;
                }

                if (currentNode.waitTime > 0f)
                {
                    isWaiting = true;
                    waitTimer = 0f;
                    return;
                }
            }

            AdvanceIndex();
            MoveToWaypoint(chef.currentWaypointIndex);
        }
    }

    public void Exit()
    {
        isWaiting = false;
    }

    private void AdvanceIndex()
    {
        // Cambiar dirección si llegamos a los extremos
        if (chef.currentWaypointIndex == 0)
            direction = 1;
        else if (chef.currentWaypointIndex == chef.waypoints.Length - 1)
            direction = -1;

        chef.currentWaypointIndex += direction;
    }

    private void MoveToWaypoint(int index)
    {
        if (chef.waypoints.Length == 0) return;

        Transform waypoint = chef.waypoints[index];
        currentNode = waypoint.GetComponent<WaypointNode>();
        chef.agent.SetDestination(waypoint.position);
    }
}
