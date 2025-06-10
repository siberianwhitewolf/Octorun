using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OctopusController : Entity, IAdjustableSpeed
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 10f;
    public bool isHiding = false;
    private bool canMove = true;
    private float hideTimer = 0;
    public float maxHideTime = 1.5f;

    private float _speedMultiplier = 1f;
    
    private Rigidbody _rb;
    private Vector3 _moveDirection;
    public MaterialFloatLerp  _materialFloatLerp;
    WallCling _wallCling;
    private OctopusJump _octopusJump;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _octopusJump  = GetComponent<OctopusJump>();
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _wallCling = GetComponent<WallCling>();
    }

    protected override void Update()
    {
        base.Update();  
        if (!_wallCling._isClinging && isAlive)
        {
            GetInputs();
            RotateTowardsMouse();
        }
            
        if (Input.GetKeyDown(KeyCode.Q) && isAlive)
        {
            SetIsHiding();
        }
        
        if (isHiding && isAlive)
        {
            HideTimer();
        }
    }
    
    void FixedUpdate()
    {
        // Movimiento directo en XZ manteniendo la velocidad en Y (por gravedad)
        if (canMove && _octopusJump.isGrounded)
        {
            Vector3 targetVelocity = _moveDirection * moveSpeed * _speedMultiplier;
            _rb.velocity = new Vector3(targetVelocity.x, _rb.velocity.y, targetVelocity.z);
        }

        
    }

    void GetInputs()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        _moveDirection = new Vector3(h, 0, v).normalized;

        // Rotar suavemente hacia la dirección de movimiento (solo eje Y)
        if (_moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 flatDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            Quaternion yOnlyRotation = Quaternion.Euler( -90, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, yOnlyRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void SetIsHiding()
    {
        isHiding = !isHiding;
        canMove = !canMove;
        _rb.velocity = new Vector3(0f, 0f, 0f);
    }

    void HideTimer()
    {
        hideTimer += Time.deltaTime;
        Debug.Log(hideTimer);
        if (hideTimer > maxHideTime)
        {
            _materialFloatLerp.triggered = true;
            hideTimer = 0;
            isHiding = !isHiding;
            canMove = !canMove;
        }
    }
    
    void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position); // plano horizontal (y = personaje)

        if (groundPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion yOnlyRotation = Quaternion.Euler(-90f, targetRotation.eulerAngles.y, 0f); // misma lógica que tu controlador
                transform.rotation = Quaternion.Slerp(transform.rotation, yOnlyRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }
    
    public void SetMovement(bool canPlayerMove)
    {
        canMove = canPlayerMove;
    
        // Si detenemos el movimiento, es buena idea resetear la velocidad del Rigidbody.
        if (!canPlayerMove)
        {
            _rb.velocity = Vector3.zero;
        }
    }
    
}