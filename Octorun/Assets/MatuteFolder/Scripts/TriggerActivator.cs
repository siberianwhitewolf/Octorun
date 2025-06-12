using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    [Tooltip("El objeto que se activar�/desactivar�")]
    public GameObject objectB;
    [Tooltip("Nombre de la capa del jugador")]
    public string playerLayerName = "Player";

    int playerLayer;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        if (objectB == null)
            Debug.LogWarning("TriggerActivator: Object B no est� asignado.", this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
            objectB.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
            objectB.SetActive(false);
    }
}
