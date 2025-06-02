//TPFinal - *Nombre y apellido del alumno*
using UnityEngine;

public class EnemyAnimatorTrigger : MonoBehaviour
{
    public Animator enemyAnimator;

    public bool isWalking;
    public bool isRunning;

    void Update()
    {
        if (enemyAnimator == null) return;

        enemyAnimator.SetBool("IsWalking", isWalking);
        enemyAnimator.SetBool("IsRunning", isRunning);
    }
}
