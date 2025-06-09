using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OctopusController))]
[RequireComponent(typeof(OctopusJump))]
public class WallCling : MonoBehaviour
{
    [Header("Configuración de Adhesión")]
    [Tooltip("La capa que identifica a las paredes a las que se puede pegar.")]
    public LayerMask wallLayer;
    [Tooltip("Distancia máxima para detectar una pared y poder pegarse.")]
    public float checkDistance = 0.7f;
    [Tooltip("Tiempo máximo en segundos que puede estar pegado.")]
    public float maxClingTime = 1.5f;

    [Header("Configuración de Salto")]
    [Tooltip("Fuerza con la que se impulsa al saltar de la pared.")]
    public float jumpForce = 10f;

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

    void Update()
    {
        if (_lockClingUntilGrounded && _octopusJump.isGrounded)
        {
            _lockClingUntilGrounded = false;
        }

        if (_isClinging)
        {
            HandleClingingState();
        }
        else
        {
            HandleDefaultState();
        }
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
        if (_clingTimer >= maxClingTime)
        {
            StopClinging();
            return;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            StopClinging();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpFromWall();
        }
    }

    private void TryToCling()
    {
        // --- CORREGIDO ---
        // Usamos el vector de movimiento hacia adelante de tu pulpo (su eje Y local) para detectar la pared.
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, checkDistance, wallLayer))
        {
            StartClinging(hit);
        }
    }

    private void StartClinging(RaycastHit wallHit)
    {
        _isClinging = true;
        _lockClingUntilGrounded = false;
        _clingTimer = 0f;
        _wallNormal = wallHit.normal; // La dirección que "sale" de la pared.

        _octopusController.SetMovement(false);
        _octopusJump.enabled = false;
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;

        // --- LÓGICA DE ROTACIÓN MODIFICADA PARA FORZAR -180 EN X ---

        // 1. Primero, calculamos la rotación ideal para que el pulpo "mire" en dirección
        //    opuesta a la pared. Esto nos dará el ángulo correcto en el eje Y.
        Quaternion lookAwayFromWall = Quaternion.LookRotation(-_wallNormal, Vector3.up);

        // 2. Extraemos únicamente el ángulo de rotación en Y de este cálculo.
        float targetYAngle = lookAwayFromWall.eulerAngles.y;

        // 3. Finalmente, creamos y aplicamos la rotación final, forzando el eje X a -180 grados
        //    y usando el ángulo Y que acabamos de obtener. Dejamos Z en 0 para evitar que el pulpo ruede.
        transform.rotation = Quaternion.Euler(-180f, targetYAngle, 0f);
    
        // El resto de la función se mantiene igual.
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
        // Primero, nos despegamos para que la física normal (gravedad, etc.) se reactive.
        StopClinging();
    
        // Activamos nuestra bandera para no volver a pegarnos al instante.
        _lockClingUntilGrounded = true;

        // --- LÍNEA CORREGIDA Y DEFINITIVA ---
        // Calculamos la dirección del salto combinando la normal de la pared (para alejarlo)
        // y el vector hacia arriba del mundo (para darle altura).
        // .normalized asegura que el vector final tenga una longitud consistente,
        // para que la fuerza del salto sea siempre la misma.
        Vector3 jumpDirection = (_wallNormal + Vector3.up).normalized;

        // Aplicamos la fuerza en la dirección calculada.
        _rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        // El Gizmo ahora también usa la dirección correcta para que veas el rayo de detección.
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * checkDistance));
    }
}