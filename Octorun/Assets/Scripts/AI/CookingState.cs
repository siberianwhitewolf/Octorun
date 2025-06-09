using UnityEngine;

public class CookingState : IState
{
    private ChefAIController chef;

    public CookingState(ChefAIController chef) => this.chef = chef;

    public void Enter()
    {
        chef.StopMovement(); // Se detiene para cocinar
        chef.animator.SetBool("IsCooking", true); // Ejemplo de animación
    }
    
    public void Update()
    {
        if (chef.lineOfSight != null && chef.lineOfSight.CanSeeTarget)
            chef.SwitchState(chef.chasingState);
        
        // Aquí podrías añadir una condición para que termine de cocinar y vaya a patrullar
    }

    public void Exit() 
    {
        chef.animator.SetBool("IsCooking", false);
    }
    
}