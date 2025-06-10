using UnityEngine;

public class OctoInputAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Grounded (opcional)")]
    [SerializeField] private bool alwaysGrounded = true;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    /* --- IDs hashados: menos GC y más rápido --- */
    static readonly int IsMovingForwardID = Animator.StringToHash("IsMovingForward");
    static readonly int StepTriggerID = Animator.StringToHash("StepTrigger");
    static readonly int JumpTriggerID = Animator.StringToHash("JumpTrigger");
    static readonly int AttackTriggerID = Animator.StringToHash("AttackTrigger");
    static readonly int AttackHeldID = Animator.StringToHash("AttackHeld");
    static readonly int HideID = Animator.StringToHash("Hide");
    static readonly int HideToggleID = Animator.StringToHash("HideToggle");
    static readonly int IsGroundedID = Animator.StringToHash("IsGrounded");

    void Update()
    {
        /* --------- Movimiento --------- */
        bool moving = Input.GetKey(KeyCode.W);
        animator.SetBool(IsMovingForwardID, moving);

        if (Input.GetKeyDown(KeyCode.W))
            animator.SetTrigger(StepTriggerID);

        /* ---------- Salto ---------- */
        if (Input.GetKeyDown(KeyCode.Space))
            animator.SetTrigger(JumpTriggerID);

        /* ---------- Ataque ---------- */
        if (Input.GetMouseButtonDown(0))
            animator.SetTrigger(AttackTriggerID);

        animator.SetBool(AttackHeldID, Input.GetMouseButton(0));

        /* -------- Ocultarse -------- */
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool nowHidden = !animator.GetBool(HideID);  // invierte estado
            animator.SetBool(HideID, nowHidden);         // memoria
            animator.SetTrigger(HideToggleID);           // pulso de 1 frame ⬅️  NUEVO
        }

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
        animator.SetBool(IsGroundedID, grounded);
    }

#if UNITY_EDITOR
    void Reset() => animator = GetComponent<Animator>();
#endif
}
