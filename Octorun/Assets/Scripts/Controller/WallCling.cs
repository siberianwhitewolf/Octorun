using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OctopusController))]
[RequireComponent(typeof(OctopusJump))]
public class WallCling : MonoBehaviour
{
    [Header("Configuración de Adhesión")]
    public LayerMask wallLayer;
    public float checkDistance = 1.0f;
    [Tooltip("Velocidad de movimiento mientras se está pegado a la pared.")]
    public float wallMoveSpeed = 3f;
    [Tooltip("Tiempo máximo en segundos que puede estar pegado.")]
    public float maxClingTime = 5f;

    [Header("Configuración de Salto desde Pared")]
    [Tooltip("La fuerza máxima del salto cargado desde la pared.")]
    public float maxWallJumpForce = 15f;
    [Tooltip("El tiempo en segundos para cargar el salto al máximo.")]
    public float chargeTimeToMax = 1.5f;

    public bool IsClinging { get; private set; } = false;

    private float _clingTimer = 0f;
    private Vector3 _wallNormal;
    private bool _lockClingUntilGrounded = false;
    private bool _isChargingJump = false;
    private float _wallJumpChargeTimer = 1f;

    private Rigidbody _rb;
    private OctopusController _octopusController;
    private OctopusJump _octopusJump;
    private Animator _animator;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _octopusController = GetComponent<OctopusController>();
        _octopusJump = GetComponent<OctopusJump>();
        _animator  = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (_lockClingUntilGrounded && _octopusJump.isGrounded)
        {
            //_lockClingUntilGrounded = false;
            _clingTimer = 0f;
        }

        if (IsClinging)
        {
            HandleClingingState();
        }
        else
        {
            // Lógica para iniciar el cling
            if (Input.GetKeyDown(KeyCode.E) && !_octopusJump.isGrounded)
            {
                TryToCling();
            }
        }
    }

    void FixedUpdate()
    {
        if (IsClinging)
        {
            HandleWallMovement();
        }
    }

    private void HandleClingingState()
    {
        // Gestión del tiempo, soltar la tecla y carga de salto
        _clingTimer += Time.deltaTime;
        if(_animator) _animator.SetBool("IsMovingForward", _rb.velocity.sqrMagnitude > 0);
        if (_clingTimer >= maxClingTime) { StopClinging(); return; }
        if (Input.GetKeyUp(KeyCode.E)) { StopClinging(); return; }

        if (Input.GetKeyDown(_octopusJump.jumpKey))
        {
            _isChargingJump = true;
            _wallJumpChargeTimer = 1f;
            if(_animator) _animator.SetBool("JumpHeld", _isChargingJump);
            if(_animator) _animator.SetTrigger("JumpTrigger");
        }

        if (_isChargingJump)
        {
            _wallJumpChargeTimer += Time.deltaTime;
        }

        if (Input.GetKeyUp(_octopusJump.jumpKey) && _isChargingJump)
        {
            if(_animator) _animator.SetBool("JumpHeld", false);
            if(_animator) _animator.SetBool("IsMovingForward", false);
            JumpFromWall();
        }
    }
    
    private void HandleWallMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * verticalInput;
        _rb.velocity = moveDirection * wallMoveSpeed;
    }

    private void TryToCling()
    {
        Vector3 directionToProbe = _octopusController.MoveDirection;
        if (directionToProbe.sqrMagnitude < 0.1f) return;
        
        Vector3 rayOrigin = transform.position + Vector3.up * 0.2f;
        if (Physics.Raycast(rayOrigin, directionToProbe, out RaycastHit hit, checkDistance, wallLayer))
        {
            StartClinging(hit);
        }
    }

    private void StartClinging(RaycastHit wallHit)
    {
        IsClinging = true;
        _animator.SetBool("ClinginWall", IsClinging);
       // _lockClingUntilGrounded = false;
       // _clingTimer = 0f;
        _wallNormal = wallHit.normal; 

        _rb.isKinematic = false;
        _rb.velocity = Vector3.zero;

        // --- ROTACIÓN CORREGIDA Y ROBUSTA ---
        // Esta es la forma correcta y estable de orientarse contra una pared.
        // 1. "Adelante" (eje Z local) apuntará en la dirección del mouse, pero proyectada sobre la pared.
        // 2. "Arriba" (eje Y local) apuntará en la dirección de la normal de la pared.
        Vector3 facingDirectionOnWall = Vector3.ProjectOnPlane(transform.forward, _wallNormal).normalized;
        transform.rotation = Quaternion.LookRotation(facingDirectionOnWall, _wallNormal);
    }

    private void StopClinging()
    {
        _lockClingUntilGrounded = true;
        IsClinging = false;
        _animator.SetBool("ClinginWall", IsClinging);
        
        _rb.isKinematic = false;
        _rb.useGravity = true;

        _isChargingJump = false;
        _wallJumpChargeTimer = 1f;
    }

    private void JumpFromWall()
    {
        float normalizedCharge = Mathf.Clamp01(_wallJumpChargeTimer / chargeTimeToMax);
        float finalForce = maxWallJumpForce * normalizedCharge;
        
        // --- LÓGICA DE SALTO CORREGIDA Y DEFINITIVA ---
        // La dirección del salto es simple: la normal de la pared (para alejarte)
        // más un poco de impulso hacia arriba del mundo (para el arco).
        // Esta lógica es en "world space" y es la más fiable.
        Vector3 jumpDirection = (_wallNormal + Vector3.up).normalized;
        
        StopClinging();

        _rb.AddForce(jumpDirection * finalForce, ForceMode.Impulse);
        _clingTimer = 0f;
    }
    
    public void RotateTowardsMouseOnWall()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane wallPlane = new Plane(-_wallNormal, transform.position); // Plano de la pared

        if (wallPlane.Raycast(ray, out float hitDist))
        {
            Vector3 hitPoint = ray.GetPoint(hitDist);
            Vector3 dirToMouse = hitPoint - transform.position;

            // 1. Proyectamos la dirección del mouse sobre el plano de la pared
            Vector3 projectedDirection = Vector3.ProjectOnPlane(dirToMouse, _wallNormal).normalized;

            if (projectedDirection.sqrMagnitude > 0.001f)
            {
                // 2. Construimos una rotación con:
                // - forward = la dirección proyectada hacia el mouse (en el plano)
                // - up = la normal de la pared (que define cómo está “pegado”)
                Quaternion targetRotation = Quaternion.LookRotation(projectedDirection, _wallNormal);
                transform.rotation = targetRotation;
            }
        }
    }

}