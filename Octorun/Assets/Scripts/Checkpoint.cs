using UnityEngine;

// Asegura que el objeto siempre tenga un collider.
[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    private void Awake()
    {
        // Forzamos a que el collider sea un trigger para que no sea un obstáculo físico.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entra en el trigger es el jugador...
        if (other.CompareTag("Player"))
        {
            // ...le decimos al CheckpointManager que este es el nuevo punto de respawn.
            CheckpointManager.Instance.SetNewCheckpoint(this.transform);

            // Para que el checkpoint solo se active una vez, lo desactivamos.
            // Esto es opcional, pero recomendado.
            gameObject.SetActive(false);
        }
    }
}