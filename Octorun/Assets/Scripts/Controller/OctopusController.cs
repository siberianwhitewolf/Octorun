using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OctopusController : MonoBehaviour, IAdjustableSpeed
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 10f;
    private bool isHiding = false;
    private bool canMove = true;

    private float _speedMultiplier = 1f;

    [Header("Visual Feedback")]
    public Transform visualBody;

    private Rigidbody _rb;
    private Vector3 _moveDirection;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    void Update()
    { 
        GetInputs();
            
        if (Input.GetKeyDown(KeyCode.Q))
        {
            setIsHiding();
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
            Quaternion yOnlyRotation = Quaternion.Euler(-90, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, yOnlyRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void FixedUpdate()
    {
        // Movimiento directo en XZ manteniendo la velocidad en Y (por gravedad)
        if (canMove)
        {
            Vector3 targetVelocity = _moveDirection * moveSpeed * _speedMultiplier;
            _rb.velocity = new Vector3(targetVelocity.x, _rb.velocity.y, targetVelocity.z);
        }
    }

    void setIsHiding()
    {
        isHiding = !isHiding;
        canMove = !canMove;
        _rb.velocity = new Vector3(0f, 0f, 0f);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }
}