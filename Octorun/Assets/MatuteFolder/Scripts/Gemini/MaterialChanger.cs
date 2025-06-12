using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class MaterialChanger : MonoBehaviour
{
    [SerializeField] private Material newMaterial;
    [SerializeField] private string layerName = "Wall";

    private int layer;
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private Collider triggerCollider;

    void Awake()
    {
        layer = LayerMask.NameToLayer(layerName);
        triggerCollider = GetComponent<Collider>();
    }

    void OnEnable()
    {
        // Si al activarse ya hay muros dentro, los cambiamos inmediatamente
        if (triggerCollider.isTrigger)
        {
            foreach (var other in Physics.OverlapBox(
                triggerCollider.bounds.center,
                triggerCollider.bounds.extents,
                transform.rotation,
                1 << layer))
            {
                ApplyChange(other);
            }
        }
    }

    void OnTriggerEnter(Collider other) => ApplyChange(other);
    void OnTriggerStay(Collider other) => ApplyChange(other);
    void OnTriggerExit(Collider other) => RestoreOriginal(other);

    void OnDisable()
    {
        // Al desactivarse, restauramos todo
        foreach (var kvp in originalMaterials)
            if (kvp.Key != null)
                kvp.Key.material = kvp.Value;
        originalMaterials.Clear();
    }

    private void ApplyChange(Collider other)
    {
        if (other.gameObject.layer != layer || newMaterial == null) return;
        var rend = other.GetComponent<Renderer>();
        if (rend != null && !originalMaterials.ContainsKey(rend))
        {
            originalMaterials[rend] = rend.material;
            rend.material = newMaterial;
        }
    }

    private void RestoreOriginal(Collider other)
    {
        if (other.gameObject.layer != layer) return;
        var rend = other.GetComponent<Renderer>();
        if (rend != null && originalMaterials.ContainsKey(rend))
        {
            rend.material = originalMaterials[rend];
            originalMaterials.Remove(rend);
        }
    }
}
