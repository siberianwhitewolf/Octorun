using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float maxJumpForce = 8f;
    public float chargeTimeToMax = 1.5f;
    public KeyCode jumpKey = KeyCode.Space;

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
        if (Input.GetKeyDown(jumpKey))
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
}
