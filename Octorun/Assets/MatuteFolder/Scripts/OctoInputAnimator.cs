using UnityEngine;

public class OctoInputAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Grounded (opcional)")]
    [SerializeField] private bool alwaysGrounded = true;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        /* --------- Movimiento --------- */
        bool moving = Input.GetKey(KeyCode.W);
        animator.SetBool("IsMovingForward", moving);

        // NUEVO ➜ dispara el ciclo con un toque
        if (Input.GetKeyDown(KeyCode.W))
            animator.SetTrigger("StepTrigger");

        /* ---------- Salto ---------- */
        if (Input.GetKeyDown(KeyCode.Space))
            animator.SetTrigger("JumpTrigger");

        /* ---------- Ataque ---------- */
        if (Input.GetMouseButtonDown(0))
            animator.SetTrigger("AttackTrigger");

        animator.SetBool("AttackHeld", Input.GetMouseButton(0));

        /* -------- Ocultarse -------- */
        if (Input.GetKeyDown(KeyCode.Q))
            animator.SetBool("Hide", !animator.GetBool("Hide"));

        /* -------- Grounded --------- */
        bool grounded = alwaysGrounded;
        if (!alwaysGrounded && groundCheck != null)
        {
            grounded = Physics.CheckSphere(
                groundCheck.position,
                groundCheckRadius,
                groundLayer,
                QueryTriggerInteraction.Ignore);
        }
        animator.SetBool("IsGrounded", grounded);
    }

#if UNITY_EDITOR
    void Reset() => animator = GetComponent<Animator>();
#endif
}
