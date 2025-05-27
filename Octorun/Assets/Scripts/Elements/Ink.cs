using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ink : MonoBehaviour
{
    public float duration = 5f;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Se queda "pegado" en la superficie
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Podrías agregar lógica para: resbalar enemigos, ralentizar NPCs, etc.
    }
}
