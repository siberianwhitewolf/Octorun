using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float maxJumpForce = 8f;
    public float chargeTimeToMax = 1.5f;
    public KeyCode jumpKey = KeyCode.Space;
    public bool isGrounded;
    
    [Header("Ground Check Settings")]
    public Transform groundCheckPoint;     // Punto de origen del chequeo (normalmente debajo del personaje)
    public float groundCheckRadius = 0.3f; // Radio de la esfera que detecta el suelo
    public LayerMask groundLayer;          // Layer que representa el suelo

    [Header("Movement Reference")]
    public MonoBehaviour movementScript; // Referencia al script que controla el movimiento
    public float slowFactor = 0.5f;

    private Rigidbody rb;
    private bool isCharging = false;
    private float chargeTimer = 0f;
    private IAdjustableSpeed adjustableSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Verifica que el script de movimiento implementa la interfaz (si corresponde)
        if (movementScript is IAdjustableSpeed)
            adjustableSpeed = (IAdjustableSpeed)movementScript;
    }

    void Update()
    {
        
        CheckGround();
        
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            isCharging = true;
            chargeTimer = 0f;

            if (adjustableSpeed != null)
                adjustableSpeed.SetSpeedMultiplier(slowFactor);
        }

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            if (chargeTimer > chargeTimeToMax)
                chargeTimer = chargeTimeToMax;
        }

        if (Input.GetKeyUp(jumpKey) && isCharging)
        {
            float normalizedCharge = chargeTimer / chargeTimeToMax;
            float finalForce = maxJumpForce * normalizedCharge;
            rb.AddForce(Vector3.up * finalForce, ForceMode.Impulse);

            isCharging = false;
            chargeTimer = 0f;

            if (adjustableSpeed != null)
                adjustableSpeed.SetSpeedMultiplier(1f); // Restaura la velocidad normal
        }
    }
    
    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
    
    
}
