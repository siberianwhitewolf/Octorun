using UnityEngine;
using System.Collections;

public class PipeTravelController : MonoBehaviour
{
    [Header("Configuración de la Ruta")]
    [Tooltip("Arrastra aquí en orden los GameObjects vacíos que marcan la ruta dentro de la tubería.")]
    public Transform[] pipeWaypoints;

    [Tooltip("Velocidad a la que se moverá el pulpo por la tubería.")]
    public float travelSpeed = 5f;

    private bool isSequenceActive = false;

    // Esta función se activa cuando un objeto con Rigidbody entra en el trigger.
    private void OnTriggerEnter(Collider other)
    {
        // Si la secuencia ya está activa o si el objeto no es el jugador, no hacemos nada.
        if (isSequenceActive || !other.CompareTag("Player"))
        {
            return;
        }

        // Si es el jugador, iniciamos la secuencia automática.
        StartCoroutine(TravelSequence(other.transform));
    }

    private IEnumerator TravelSequence(Transform playerOctopus)
    {
        isSequenceActive = true;

        // 1. Obtenemos los componentes del jugador para desactivarlos.
        OctopusController controller = playerOctopus.GetComponent<OctopusController>();
        OctopusJump jump = playerOctopus.GetComponent<OctopusJump>();
        WallCling cling = playerOctopus.GetComponent<WallCling>();
        Rigidbody rb = playerOctopus.GetComponent<Rigidbody>();

        // 2. Desactivamos el control del jugador y la física.
        if (controller) controller.enabled = false;
        if (jump) jump.enabled = false;
        if (cling) cling.enabled = false;
        if (rb)
        {
            rb.isKinematic = true; // Hacemos que ignore la física para que no choque.
            rb.velocity = Vector3.zero;
        }

        // 3. Movemos al pulpo a través de cada waypoint de la tubería.
        foreach (Transform waypoint in pipeWaypoints)
        {
            // Mientras no hayamos llegado al waypoint actual...
            while (Vector3.Distance(playerOctopus.position, waypoint.position) > 0.1f)
            {
                // ...movemos al pulpo hacia él.
                playerOctopus.position = Vector3.MoveTowards(playerOctopus.position, waypoint.position, travelSpeed * Time.deltaTime);

                // Hacemos que el pulpo rote suavemente para mirar hacia el siguiente punto.
                Vector3 direction = (waypoint.position - playerOctopus.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    playerOctopus.rotation = Quaternion.Slerp(playerOctopus.rotation, targetRotation, Time.deltaTime * 5f);
                }
                
                yield return null; // Esperamos al siguiente frame.
            }
        }

        // 4. Una vez que el bucle termina, hemos llegado al último waypoint.
        Debug.Log("El pulpo ha llegado al final de la tubería.");

        // Buscamos si el último waypoint tiene un componente Node para disparar su evento.
        Node finalNode = pipeWaypoints[pipeWaypoints.Length - 1].GetComponent<Node>();
        if (finalNode != null && finalNode.OnNodeReached != null)
        {
            Debug.Log("Disparando evento del nodo final...");
            finalNode.OnNodeReached.Invoke(); // ¡Aquí se dispara el evento!
        }

        // No reactivamos el control porque la escena va a cambiar.
    }
}