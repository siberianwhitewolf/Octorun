//TPFinal - *Nombre y apellido del alumno*
using UnityEngine;

public class EnemyAnimatorTrigger : MonoBehaviour
{
    [Header("References")]
    public Animator enemyAnimator;

    [Header("Animation Flags (editable in Inspector)")]
    public bool isWalking;
    public bool isRunning;
    [Tooltip("Marcar en TRUE para disparar el ataque. Se vuelve a FALSE automáticamente.")]
    public bool attackTrigger;               // ← variable de disparo para Trigger

    /* --- Hashes opcionales (rendimiento) ---
    static readonly int IsWalkingID    = Animator.StringToHash("IsWalking");
    static readonly int IsRunningID    = Animator.StringToHash("IsRunning");
    static readonly int IsAttackingID  = Animator.StringToHash("IsAttacking");
    */

    void Update()
    {
        if (enemyAnimator == null) return;

        enemyAnimator.SetBool("IsWalking", isWalking);   // o SetBool(IsWalkingID, …)
        enemyAnimator.SetBool("IsRunning", isRunning);   // o SetBool(IsRunningID, …)

        // --- Disparo del Trigger ---
        if (attackTrigger)
        {
            enemyAnimator.SetTrigger("IsAttacking");     // o SetTrigger(IsAttackingID)
            attackTrigger = false;                       // se auto-resetea
        }
    }
}
