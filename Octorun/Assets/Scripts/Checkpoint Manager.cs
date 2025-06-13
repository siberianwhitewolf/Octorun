using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // Singleton Pattern: Una forma de tener una única instancia global de este manager,
    // accesible desde cualquier otro script de forma sencilla.
    public static CheckpointManager Instance { get; private set; }

    // Guardaremos el Transform del último checkpoint activo.
    public Transform CurrentCheckpoint { get; private set; }

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Al empezar el juego, si no se ha definido ningún checkpoint,
        // el punto de partida del jugador será el checkpoint inicial por defecto.
        if (CurrentCheckpoint == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CurrentCheckpoint = player.transform;
                Debug.Log("Checkpoint inicial establecido en la posición del jugador.");
            }
        }
    }

    /// <summary>
    /// Los checkpoints individuales llamarán a esta función para establecerse como el nuevo punto de respawn.
    /// </summary>
    public void SetNewCheckpoint(Transform newCheckpointTransform)
    {
        // Comparamos para no actualizar innecesariamente si se pasa por el mismo checkpoint de nuevo.
        if (CurrentCheckpoint != newCheckpointTransform)
        {
            CurrentCheckpoint = newCheckpointTransform;
            Debug.Log($"Nuevo checkpoint activado: {newCheckpointTransform.name}");
            // Aquí podrías añadir un sonido o efecto visual de "punto de guardado".
        }
    }
}