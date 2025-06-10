using UnityEngine;

public class OctoAnimationTester : MonoBehaviour
{
    [Header("Asignar Animator")]
    public Animator animator;

    [Header("Teclas configurables")]
    public KeyCode toggleHideKey = KeyCode.Q;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode loopAttackKey = KeyCode.Mouse1;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode fallKey = KeyCode.F;

    [Header("Teclas de movimiento")]
    public KeyCode moveStartKey = KeyCode.Alpha1;
    public KeyCode moveStopKey = KeyCode.Alpha2;

    [Header("Estados de grounded")]
    public KeyCode setGroundedKey = KeyCode.Alpha3;
    public KeyCode setAirborneKey = KeyCode.Alpha4;

    private bool isHidden = false;

    void Update()
    {
        if (animator == null) return;

        // Toggle esconderse/desesconderse con Q
        if (Input.GetKeyDown(toggleHideKey))
        {
            if (isHidden)
            {
                animator.SetTrigger("Unhide");
                isHidden = false;
                Debug.Log("Unhide");
            }
            else
            {
                animator.SetTrigger("StartHide");
                isHidden = true;
                Debug.Log("Hide");
            }
        }

        // Ataque
        if (Input.GetKeyDown(attackKey)) animator.SetTrigger("AttackStart");

        // Loop de ataque
        if (Input.GetKeyDown(loopAttackKey)) animator.SetBool("IsAttacking", true);
        if (Input.GetKeyUp(loopAttackKey)) animator.SetBool("IsAttacking", false);

        // Movimiento
        if (Input.GetKeyDown(moveStartKey)) animator.SetBool("IsMoving", true);
        if (Input.GetKeyDown(moveStopKey)) animator.SetBool("IsMoving", false);

        // Suelo / Aire
        if (Input.GetKeyDown(setGroundedKey)) animator.SetBool("IsGrounded", true);
        if (Input.GetKeyDown(setAirborneKey)) animator.SetBool("IsGrounded", false);

        // Saltar / caer
        if (Input.GetKeyDown(jumpKey)) animator.SetTrigger("JumpAnticipation");
        if (Input.GetKeyDown(fallKey)) animator.SetTrigger("JumpFall");

        // VerticalSpeed (testing)
        if (Input.GetKeyDown(KeyCode.Alpha5)) animator.SetFloat("VerticalSpeed", 1f);     // subiendo
        if (Input.GetKeyDown(KeyCode.Alpha6)) animator.SetFloat("VerticalSpeed", 0.1f);   // suspendido
        if (Input.GetKeyDown(KeyCode.Alpha7)) animator.SetFloat("VerticalSpeed", -1f);    // cayendo
    }
}
