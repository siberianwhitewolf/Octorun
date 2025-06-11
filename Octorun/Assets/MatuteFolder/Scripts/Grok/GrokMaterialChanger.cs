using UnityEngine;
using System.Collections.Generic; // Added for Dictionary

public class GrokMaterialChanger : MonoBehaviour
{
    [SerializeField] private Material newMaterial; // Material to apply, set in Inspector
    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (collider == null) { Debug.LogError("No Collider on " + gameObject.name); }
        if (rb == null) { rb = gameObject.AddComponent<Rigidbody>(); rb.isKinematic = true; }
        if (newMaterial == null) { Debug.LogError("No material assigned in Inspector for " + gameObject.name); }
        if (LayerMask.NameToLayer("Wall") == -1) { Debug.LogError("Layer 'Wall' does not exist."); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Renderer renderer = collision.gameObject.GetComponent<Renderer>();
            if (renderer != null && newMaterial != null)
            {
                if (!originalMaterials.ContainsKey(collision.gameObject))
                {
                    originalMaterials[collision.gameObject] = renderer.material;
                }
                renderer.material = newMaterial;
                Debug.Log("Changed material on " + collision.gameObject.name);
            }
            else
            {
                Debug.LogWarning("No Renderer or material on " + collision.gameObject.name);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Renderer renderer = collision.gameObject.GetComponent<Renderer>();
            if (renderer != null && originalMaterials.ContainsKey(collision.gameObject))
            {
                renderer.material = originalMaterials[collision.gameObject];
                originalMaterials.Remove(collision.gameObject);
                Debug.Log("Restored material on " + collision.gameObject.name);
            }
        }
    }
}
