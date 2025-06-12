using UnityEngine;
using System.Collections;

public class JumpPad : MonoBehaviour
{
    [Tooltip("Fuerza de salto aplicado al jugador")]
    public float jumpForce = 10f;

    [Tooltip("Tiempo en segundos antes de poder reactivar el pad")]
    public float cooldown = 1f;

    private bool ready = true;
    private int playerLayer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!ready || collision.gameObject.layer != playerLayer) return;

        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StartCoroutine(CooldownRoutine());
        }
    }

    IEnumerator CooldownRoutine()
    {
        ready = false;
        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}
