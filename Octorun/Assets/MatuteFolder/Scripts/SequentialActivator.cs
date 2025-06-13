using UnityEngine;
using System.Collections;

public class SequentialActivator : MonoBehaviour
{
    [Tooltip("Tag del objeto que activa la secuencia")]
    public string triggeringTag = "Player";
    [Tooltip("Tres objetos que se activarán/desactivarán secuencialmente")]
    public GameObject[] targets = new GameObject[3];
    [Tooltip("Segundos que cada objeto permanece activo e inactivo")]
    public float interval = 1f;

    private bool hasTriggered;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag(triggeringTag)) return;
        hasTriggered = true;
        StartCoroutine(ActivateSequence());
    }

    IEnumerator ActivateSequence()
    {
        foreach (var target in targets)
        {
            target.SetActive(true);
            yield return new WaitForSeconds(interval);
            target.SetActive(false);
            yield return new WaitForSeconds(interval);
        }
        // Apaga este objeto al terminar
        gameObject.SetActive(false);
    }
}
