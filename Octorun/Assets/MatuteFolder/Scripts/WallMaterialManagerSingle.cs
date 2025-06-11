using UnityEngine;
using System.Collections.Generic;

public class WallMaterialManagerSingle : MonoBehaviour
{
    [SerializeField] private Material targetMaterial; // Material a asignar a la pared
    [SerializeField] private Transform player; // Referencia al jugador
    [SerializeField] private LayerMask wallLayer; // Capa para las paredes
    private Camera mainCamera; // Referencia a la c�mara principal
    private Dictionary<Renderer, Material> originalMaterials; // Almacena materiales originales
    private Renderer lastAffectedRenderer; // Rastrea la �ltima pared afectada

    void Start()
    {
        // Obtener la c�mara principal
        mainCamera = Camera.main;
        // Inicializar el diccionario para materiales originales
        originalMaterials = new Dictionary<Renderer, Material>();
        // Inicializar la referencia del �ltimo renderer afectado
        lastAffectedRenderer = null;
    }

    void Update()
    {
        // Asegurarse de que tenemos todas las referencias
        if (player == null || mainCamera == null)
            return;

        // Restaurar el material de la �ltima pared afectada, si existe
        if (lastAffectedRenderer != null)
        {
            if (originalMaterials.ContainsKey(lastAffectedRenderer))
            {
                lastAffectedRenderer.material = originalMaterials[lastAffectedRenderer];
            }
            lastAffectedRenderer = null;
        }

        // Obtener la posici�n de la c�mara y del jugador
        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 playerPos = player.position;

        // Direcci�n del rayo desde la c�mara al jugador
        Vector3 direction = playerPos - cameraPos;
        float distance = direction.magnitude;

        // Lanzar el raycast para detectar solo el primer objeto en la capa "Wall"
        Ray ray = new Ray(cameraPos, direction);
        RaycastHit hit;

        // Comprobar si el raycast golpea un objeto en la capa "Wall"
        if (Physics.Raycast(ray, out hit, distance, wallLayer))
        {
            Renderer hitRenderer = hit.transform.GetComponent<Renderer>();
            if (hitRenderer != null)
            {
                // Guardar el material original si no est� en el diccionario
                if (!originalMaterials.ContainsKey(hitRenderer))
                {
                    originalMaterials[hitRenderer] = hitRenderer.material;
                }
                // Aplicar el material objetivo solo a este objeto
                hitRenderer.material = targetMaterial;
                // Rastrear este renderer como el �ltimo afectado
                lastAffectedRenderer = hitRenderer;
            }
        }

        // Depuraci�n: visualizar el rayo en la escena
        Debug.DrawRay(cameraPos, direction, Color.red);
    }
}
