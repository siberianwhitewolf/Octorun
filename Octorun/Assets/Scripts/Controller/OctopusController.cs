using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OctopusController : Entity, IAdjustableSpeed // Asegúrate que tu clase herede de Entity si lo necesitas
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Puedes ajustar la velocidad
    public float rotationSpeed = 15f;

    [Header("Hiding Settings")]
    public bool isHiding = false;
    private bool canMove = true;
    private float hideTimer = 0;
    public float maxHideTime = 1.5f;

    private float _speedMultiplier = 1f;

    private Rigidbody _rb;
    private Vector3 _moveDirection;
    public MaterialFloatLerp  _materialFloatLerp;
    private WallCling _wallCling;
    private OctopusJump _octopusJump;
    private Animator _animator;

    // Referencia a la cámara principal para el movimiento isométrico
    private Camera mainCamera;
    public Vector3 MoveDirection { get; private set; }

    protected override void Awake()
    {
        base.Awake(); // Llama al Awake de Entity si heredas de él
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _octopusJump = GetComponent<OctopusJump>();
        _wallCling = GetComponent<WallCling>();
        mainCamera = Camera.main; // Guardamos la referencia a la cámara principal
    }

    protected override void Start()
    {
        base.Start();
        if (_rb != null)
        {
            _rb.freezeRotation = true;
        }
    }

    protected override void Update()
    {
        base.Update();
        
        // Comprobamos si podemos movernos (no estamos escondidos, pegados a la pared, etc.)
        bool canCurrentlyMove = !isHiding && (_wallCling == null || !_wallCling._isClinging) && isAlive;
        
        if (canCurrentlyMove)
        {
            GetInputs();
            RotateTowardsMouse();
        }
        else
        {
            // Si no nos podemos mover, aseguramos que la dirección sea cero.
            _moveDirection = Vector3.zero;
        }
            
        if (Input.GetKeyDown(KeyCode.Q) && isAlive)
        {
            _animator.SetTrigger("HideToggle");
            SetIsHiding();
        }
        
        if (isHiding && isAlive)
        {
            HideTimer();
        }
    }
    
    void FixedUpdate()
    {
        // La lógica de movimiento ahora es más simple, solo aplica la dirección calculada.
        if (canMove && _octopusJump.isGrounded)
        {
            // Aplicamos la velocidad. Si no hay input, _moveDirection será cero.
            Vector3 targetVelocity = _moveDirection * moveSpeed * _speedMultiplier;
            _rb.velocity = new Vector3(targetVelocity.x, _rb.velocity.y, targetVelocity.z);
        }
    }

    /// <summary>
    /// Calcula la dirección del movimiento basándose en la rotación de la cámara.
    /// </summary>
    void GetInputs()
    {
        float h = Input.GetAxis("Horizontal"); // A/D o Izquierda/Derecha
        float v = Input.GetAxis("Vertical");   // W/S o Arriba/Abajo

        // --- LÓGICA DE MOVIMIENTO ISOMÉTRICO ---

        // 1. Obtenemos los vectores de dirección de la cámara.
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        // 2. Aplanamos los vectores para que no tengan componente vertical (Y).
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Creamos la dirección de movimiento final combinando el input del jugador
        //    con las direcciones de la cámara.
        //    'v' (W/S) controla el movimiento a lo largo del eje "adelante" de la cámara.
        //    'h' (A/D) controla el movimiento a lo largo del eje "derecha" de la cámara.
        _moveDirection = (camForward * v + camRight * h);
        _animator.SetBool("IsMovingForward", _moveDirection.sqrMagnitude > 0);

        // Opcional: Normalizar si se usa teclado para que el movimiento diagonal no sea más rápido.
        if (_moveDirection.sqrMagnitude > 1f)
        { 
            _moveDirection.Normalize();
            MoveDirection = _moveDirection;
        }
    }
    
    /// <summary>
    /// Rota al personaje para que siempre mire hacia la posición del cursor del mouse.
    /// </summary>
    void RotateTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        // Creamos un plano a la altura del personaje para intersectar el rayo.
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            Vector3 directionToLook = hitPoint - transform.position;
            directionToLook.y = 0f; // Nos aseguramos de que la rotación sea solo en el eje Y.

            if (directionToLook.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                
                // Aplicamos la rotación base de -90 en X de tu modelo si es necesario.
                // Si tu modelo ya mira hacia adelante por defecto, puedes usar solo 'targetRotation'.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * Quaternion.Euler(-90, 0, 0), Time.deltaTime * rotationSpeed);
            }
        }
    }
    
    // --- El resto de tus funciones se mantienen igual ---

    void SetIsHiding()
    {
        isHiding = !isHiding;
        canMove = !isHiding;
        _animator.SetBool("Hide", isHiding);
        if(isHiding) _rb.velocity = Vector3.zero;
    }

    void HideTimer()
    {
        hideTimer += Time.deltaTime;
        if (hideTimer > maxHideTime)
        {
            if(_materialFloatLerp != null) _materialFloatLerp.triggered = true;
            hideTimer = 0;
            SetIsHiding();
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }
    
    public void SetMovement(bool canPlayerMove)
    {
        canMove = canPlayerMove;
        if (!canPlayerMove)
        {
            _rb.velocity = Vector3.zero;
        }
    }
}