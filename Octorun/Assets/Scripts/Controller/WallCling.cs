using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OctopusController))]
[RequireComponent(typeof(OctopusJump))]
public class WallCling : MonoBehaviour
{
    [Header("Configuración de Adhesión")]
    public LayerMask wallLayer;
    public float checkDistance = 0.7f;
    public float maxClingTime = 1.5f;

    [Header("Configuración de Salto")]
    public float jumpForce = 12f; // Puedes ajustar esta fuerza

    private Rigidbody _rb;
    private OctopusController _octopusController;
    private OctopusJump _octopusJump;

    public bool _isClinging = false;
    private float _clingTimer = 0f;
    private Vector3 _wallNormal;

    private bool _lockClingUntilGrounded = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _octopusController = GetComponent<OctopusController>();
        _octopusJump = GetComponent<OctopusJump>();
    }

    // ... (Update y HandleStates se mantienen igual) ...
    void Update()
    {
        if (_lockClingUntilGrounded && _octopusJump.isGrounded)
        {
            _lockClingUntilGrounded = false;
        }

        if (_isClinging) { HandleClingingState(); }
        else { HandleDefaultState(); }
    }
    private void HandleDefaultState()
    {
        if (Input.GetKey(KeyCode.E) && !_octopusJump.isGrounded && !_lockClingUntilGrounded)
        {
            TryToCling();
        }
    }
    private void HandleClingingState()
    {
        _clingTimer += Time.deltaTime;
        if (_clingTimer >= maxClingTime) { StopClinging(); return; }
        if (Input.GetKeyUp(KeyCode.E)) { StopClinging(); return; }
        if (Input.GetKeyDown(KeyCode.Space)) { JumpFromWall(); }
    }


    private void TryToCling()
    {
        // --- LÓGICA DE DETECCIÓN CORREGIDA ---
        // Le preguntamos al controlador la dirección del input del jugador.
        Vector3 directionToProbe = _octopusController.MoveDirection;

        // Si el jugador no se está moviendo, no intentamos pegarnos.
        if (directionToProbe.sqrMagnitude < 0.1f) return;
        
        // Usamos esa dirección para lanzar el rayo. ¡Ahora siempre será la correcta!
        if (Physics.Raycast(transform.position, directionToProbe, out RaycastHit hit, checkDistance, wallLayer))
        {
            StartClinging(hit);
        }
    }

    private void StartClinging(RaycastHit wallHit)
    {
        _isClinging = true;
        _lockClingUntilGrounded = false;
        _clingTimer = 0f;
        _wallNormal = wallHit.normal; 

        _octopusController.SetMovement(false);
        _octopusJump.enabled = false;
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;

        // La lógica de rotación que funcionaba la mantenemos.
        Quaternion lookAwayFromWall = Quaternion.LookRotation(-_wallNormal, Vector3.up);
        float targetYAngle = lookAwayFromWall.eulerAngles.y;
        transform.rotation = Quaternion.Euler(-180f, targetYAngle, 0f);
    
        transform.position = wallHit.point;
    }

    private void StopClinging()
    {
        _lockClingUntilGrounded = true;
        _isClinging = false;
        _octopusController.SetMovement(true);
        _octopusJump.enabled = true;
        _rb.useGravity = true;
    }

    private void JumpFromWall()
    {
        // Esta función ahora está más limpia.
        StopClinging();
    
        // --- LÓGICA DE SALTO CORREGIDA Y DEFINITIVA ---
        // Volvemos a la lógica que funcionaba. Ahora el _wallNormal será el correcto
        // porque el Raycast en TryToCling golpeará la pared de frente.
        Vector3 jumpDirection = (_wallNormal + Vector3.up).normalized;
        Debug.Log(jumpDirection);

        _rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        // El Gizmo ahora muestra la dirección de movimiento del controlador.
        Gizmos.color = Color.cyan;
        if (_octopusController != null)
        {
            Gizmos.DrawLine(transform.position, transform.position + (_octopusController.MoveDirection * checkDistance));
        }
    }
}