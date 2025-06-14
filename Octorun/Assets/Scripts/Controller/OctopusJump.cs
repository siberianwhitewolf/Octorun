using System;
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
    public Transform groundCheckPoint; // Punto de origen del chequeo (normalmente debajo del personaje)
    public float groundCheckRadius = 0.3f; // Radio de la esfera que detecta el suelo
    public LayerMask groundLayer; // Layer que representa el suelo

    [Header("Movement Reference")]
    public MonoBehaviour movementScript; // Referencia al script que controla el movimiento
    public float slowFactor = 0.5f;
    public Entity playerEntity;

    private Rigidbody _rb;
    private bool _isCharging = false;
    private float _chargeTimer = 1f;
    private IAdjustableSpeed _adjustableSpeed;
    Animator _animator;
    private WallCling wallCling;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        wallCling = GetComponent<WallCling>();

        // Verifica que el script de movimiento implementa la interfaz (si corresponde)
        if (movementScript is IAdjustableSpeed speed)
            _adjustableSpeed = speed;
    }

    void Update()
    {
        
        if (wallCling != null && wallCling.IsClinging) return;
        
        CheckGround();
        
        if (Input.GetKeyDown(jumpKey) && isGrounded && playerEntity.IsAlive)
        {
            _isCharging = true;
            _animator.SetBool("JumpHeld", _isCharging);
            _animator.SetTrigger("JumpTrigger");

            if (_adjustableSpeed != null)
                _adjustableSpeed.SetSpeedMultiplier(slowFactor);
        }

        if (_isCharging)
        {
            _chargeTimer += Time.deltaTime;

            if (_chargeTimer > chargeTimeToMax)
                _chargeTimer = chargeTimeToMax;
        }

        if (Input.GetKeyUp(jumpKey) && _isCharging)
        {
            float normalizedCharge = _chargeTimer / chargeTimeToMax;
            float finalForce = maxJumpForce * normalizedCharge;
            _rb.AddForce(Vector3.up * finalForce, ForceMode.Impulse);

            _isCharging = false;
            _chargeTimer = 1f;
            _animator.SetBool("JumpHeld", _isCharging);

            if (_adjustableSpeed != null)
                _adjustableSpeed.SetSpeedMultiplier(1f); // Restaura la velocidad normal
        }
    }
    
    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        _animator.SetBool("IsGrounded", isGrounded);
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
