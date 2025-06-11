using UnityEngine;

/// TPFinal - * Nombre y apellido del alumno *
/// Asigna este script a un GameObject vacío con un Collider y un Rigidbody (marcado como kinematic).
/// Cuando colisione con cualquier objeto en la layer “wall”, reemplazará sus materiales por el
/// material asignado desde el Inspector.

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WallMaterialChanger : MonoBehaviour
{
    [SerializeField] private Material newMaterial;     // Material a aplicar

    private int wallLayer;

    private void Awake()
    {
        wallLayer = LayerMask.NameToLayer("wall");
        GetComponent<Rigidbody>().isKinematic = true;  // Evita que el objeto se mueva por físicas
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != wallLayer || newMaterial == null) return;

        foreach (Renderer rend in collision.gameObject.GetComponentsInChildren<Renderer>())
        {
            var mats = rend.materials;                 // Copia local para no modificar sharedMaterial
            for (int i = 0; i < mats.Length; i++)
                mats[i] = newMaterial;
            rend.materials = mats;
        }
    }
}
