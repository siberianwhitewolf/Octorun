using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FallRespawnTrigger : MonoBehaviour
{
    [Tooltip("La cantidad de daño que recibe el jugador al caer.")]
    public int fallDamage = 1;

    private void Awake()
    {
        // Aseguramos que sea un trigger.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que cae es el jugador...
        if (other.CompareTag("Player"))
        {
            // Intentamos obtener su componente Entity.
            Entity playerEntity = other.GetComponent<Entity>();
            if (playerEntity != null && playerEntity.isAlive)
            {
                Debug.Log("Jugador ha caído al vacío. Aplicando daño y reapareciendo.");

                // 1. Aplicamos el daño.
                playerEntity.TakeDamage(fallDamage);

                // 2. Si el jugador sigue vivo después del daño, lo teletransportamos.
                //    (Si el daño lo mata, el script Entity ya se encarga de la pantalla de derrota).
                if (playerEntity.isAlive)
                {
                    // Obtenemos la posición del último checkpoint desde el manager.
                    Vector3 respawnPoint = CheckpointManager.Instance.CurrentCheckpoint.position;

                    // Teletransportamos al jugador.
                    other.transform.position = respawnPoint;

                    // Reseteamos la velocidad del Rigidbody para que no caiga con el impulso anterior.
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
    }
}