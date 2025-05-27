using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OctopusController : MonoBehaviour, IAdjustableSpeed
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float hopForce = 3f;
    public float hopCooldown = 0.4f;
    public float rotationSpeed = 10f;

    private float _speedMultiplier = 1f;

    [Header("Visual Feedback")]
    public Transform visualBody;

    private Rigidbody _rb;
    private Vector3 _moveDirection;
    private float _hopTimer;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _hopTimer = 0f;

        if (visualBody == null)
            visualBody = transform;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        _moveDirection = new Vector3(h, 0, v).normalized;

        // Rotar suavemente hacia la dirección de movimiento (solo eje Y)
        if (_moveDirection.magnitude > 0.1f)
        {
            Vector3 flatDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            Quaternion yOnlyRotation = Quaternion.Euler(-90, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, yOnlyRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void FixedUpdate()
    {
        _hopTimer -= Time.fixedDeltaTime;

        if (_moveDirection.magnitude > 0.1f && _hopTimer <= 0f)
        {
            // La velocidad horizontal es afectada por el multiplicador, el salto vertical no
            Vector3 hopVector = _moveDirection * (moveSpeed * _speedMultiplier) + Vector3.up * hopForce;
            Debug.Log(hopVector);
            _rb.AddForce(hopVector, ForceMode.Impulse);
            _hopTimer = hopCooldown;
        }

        // Freno suave si no hay input
        if (_moveDirection.magnitude < 0.1f)
        {
            _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, Time.fixedDeltaTime * 5f);
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }
}
