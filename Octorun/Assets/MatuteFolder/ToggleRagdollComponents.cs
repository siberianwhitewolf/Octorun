//TPFinal - *Nombre y apellido del alumno*
using UnityEngine;

public class ToggleRagdollComponents : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;
    private CharacterJoint[] joints;
    private bool isEnabled = false;

    void Start()
    {
        // Detecta todos los componentes en hijos y subhijos
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        joints = GetComponentsInChildren<CharacterJoint>(true);

        // Desactiva todo al iniciar
        SetComponentsEnabled(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isEnabled = !isEnabled;
            SetComponentsEnabled(isEnabled);
        }
    }

    void SetComponentsEnabled(bool state)
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = !state; // Kinematic cuando está apagado
            rb.detectCollisions = state;
        }

        foreach (var col in colliders)
        {
            col.enabled = state;
        }

        // Alternativamente podés desactivar todo el GameObject si querés apagar completamente el joint
        foreach (var joint in joints)
        {
            joint.gameObject.SetActive(state);
        }
    }
}
