using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OctopusController : Entity, IAdjustableSpeed
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
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

    [Header("References")]
    public Animator _animator; 
    private Camera mainCamera;
    public Vector3 MoveDirection { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _octopusJump = GetComponent<OctopusJump>();
        _wallCling = GetComponent<WallCling>();
        mainCamera = Camera.main;
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
        if (_wallCling != null && _wallCling.IsClinging)
        {
            _moveDirection = Vector3.zero; 
            animator.SetBool("IsMovingForward", false);
            _wallCling.RotateTowardsMouseOnWall();
            return;
        }

        base.Update();

        bool canCurrentlyMove = !isHiding && (_wallCling == null || !_wallCling.IsClinging) && isAlive;

        if (canCurrentlyMove)
        {
            GetInputs();
            RotateTowardsMouse();
        }
        else
        {
            _moveDirection = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.Q) && isAlive)
        {
            if(animator) animator.SetTrigger("HideToggle");
            SetIsHiding();
        }

        if (isHiding && isAlive)
        {
            HideTimer();
        }

        if (animator != null)
        {
            animator.SetBool("IsMovingForward", _moveDirection.sqrMagnitude > 0);
        }
    }

    void FixedUpdate()
    {
        if (canMove && _octopusJump.isGrounded)
        {
            Vector3 targetVelocity = _moveDirection * moveSpeed * _speedMultiplier;
            _rb.velocity = new Vector3(targetVelocity.x, _rb.velocity.y, targetVelocity.z);
        }
    }

    void GetInputs()
    {
        float verticalInput = Input.GetAxis("Vertical");
        _moveDirection = transform.forward * verticalInput;

        if (_moveDirection.sqrMagnitude > 0)
        {
            MoveDirection = _moveDirection.normalized;
        }
        else
        {
            MoveDirection = Vector3.zero;
        }
    }

    void RotateTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            Vector3 directionToLook = hitPoint - transform.position;
            directionToLook.y = 0f;

            if (directionToLook.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

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