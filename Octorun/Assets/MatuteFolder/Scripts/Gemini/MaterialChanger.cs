using UnityEngine;
using System.Collections.Generic; // Necesario para Dictionary

public class MaterialChanger : MonoBehaviour
{
    [SerializeField]
    private Material nuevoMaterial;

    [SerializeField]
    private string layerAfectada = "Wall";

    // Diccionario para almacenar los materiales originales de los objetos que colisionan
    private Dictionary<GameObject, Material> materialesOriginales = new Dictionary<GameObject, Material>();

    void Start()
    {
        Debug.Log("MaterialChanger: Script iniciado en el objeto: " + gameObject.name);
        if (nuevoMaterial == null)
        {
            Debug.LogError("MaterialChanger: ¡ADVERTENCIA! El 'Nuevo Material' no está asignado en el Inspector.");
        }
        if (LayerMask.NameToLayer(layerAfectada) == -1)
        {
            Debug.LogError("MaterialChanger: ¡ADVERTENCIA! La capa '" + layerAfectada + "' no existe en Project Settings -> Layers.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER DETECTADO con el objeto: " + other.gameObject.name + " desde: " + gameObject.name);
        Debug.Log("La capa REAL de '" + other.gameObject.name + "' es: " + LayerMask.LayerToName(other.gameObject.layer));
        Debug.Log("La capa ESPERADA es: " + layerAfectada + " (valor numérico: " + LayerMask.NameToLayer(layerAfectada) + ")");

        // Comprobar si la capa del objeto colisionado coincide
        if (other.gameObject.layer == LayerMask.NameToLayer(layerAfectada))
        {
            Debug.Log("CAPA CORRECTA. Intentando cambiar el material...");
            Renderer rendererObjeto = other.gameObject.GetComponent<Renderer>();

            if (rendererObjeto != null && nuevoMaterial != null)
            {
                // Almacenar el material original antes de cambiarlo, si no lo hemos guardado ya
                if (!materialesOriginales.ContainsKey(other.gameObject))
                {
                    materialesOriginales.Add(other.gameObject, rendererObjeto.material);
                    Debug.Log("Material original de " + other.gameObject.name + " guardado.");
                }

                rendererObjeto.material = nuevoMaterial;
                Debug.Log("¡Material de " + other.gameObject.name + " cambiado con éxito a " + nuevoMaterial.name + "!");
            }
            else
            {
                Debug.LogError("ERROR: No se encontró el componente Renderer en " + other.gameObject.name + " o el Nuevo Material no está asignado en " + gameObject.name + ".");
            }
        }
        else
        {
            Debug.LogWarning("La capa de " + other.gameObject.name + " (" + LayerMask.LayerToName(other.gameObject.layer) + ") no es '" + layerAfectada + "'. No se cambiará el material.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TRIGGER FINALIZADO con el objeto: " + other.gameObject.name + " desde: " + gameObject.name);

        // Verificar si el objeto con el que se terminó el trigger estaba en la capa afectada
        if (other.gameObject.layer == LayerMask.NameToLayer(layerAfectada))
        {
            // Si tenemos el material original guardado para este objeto, restaurarlo
            if (materialesOriginales.ContainsKey(other.gameObject))
            {
                Renderer rendererObjeto = other.gameObject.GetComponent<Renderer>();
                if (rendererObjeto != null)
                {
                    rendererObjeto.material = materialesOriginales[other.gameObject];
                    materialesOriginales.Remove(other.gameObject); // Eliminar del diccionario después de restaurar
                    Debug.Log("Material de " + other.gameObject.name + " restaurado a su estado original.");
                }
                else
                {
                    Debug.LogError("ERROR: No se encontró el componente Renderer al intentar restaurar el material de " + other.gameObject.name + ".");
                }
            }
        }
    }
}