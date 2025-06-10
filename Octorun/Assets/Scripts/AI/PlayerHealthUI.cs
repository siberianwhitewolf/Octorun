using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con componentes de UI como Image y Layout Groups.

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Configuración de UI")]
    [Tooltip("El Prefab de la imagen del pulpo que representa un punto de vida.")]
    public GameObject octopusIconPrefab;

    [Tooltip("El objeto contenedor en el Canvas donde se crearán los iconos (ej. un Horizontal Layout Group).")]
    public Transform iconContainer;

    // Lista privada para mantener un registro de los iconos que hemos creado y poder borrarlos.
    private List<GameObject> activeIcons = new List<GameObject>();

    /// <summary>
    /// Al activarse este componente, se suscribe al evento de cambio de vida del jugador.
    /// Esto asegura que siempre estemos "escuchando" los cambios.
    /// </summary>
    private void OnEnable()
    {
        // Nos suscribimos a nuestro evento estático de la clase Entity.
        // Ahora, cada vez que la vida del jugador cambie, se llamará a nuestra función UpdateHealthDisplay.
        Entity.OnPlayerHealthChanged += UpdateHealthDisplay;
    }

    /// <summary>
    /// Al desactivarse, nos desuscribimos para evitar errores y fugas de memoria.
    /// Es una buena práctica de programación.
    /// </summary>
    private void OnDisable()
    {
        Entity.OnPlayerHealthChanged -= UpdateHealthDisplay;
    }

    /// <summary>
    /// Esta función se ejecuta automáticamente cuando la vida del jugador cambia,
    /// porque está suscrita al evento OnPlayerHealthChanged.
    /// </summary>
    /// <param name="currentHealth">La vida actual que nos llega desde el evento.</param>
    /// <param name="maxHealth">La vida máxima (no la usamos aquí, pero el evento la envía).</param>
    private void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        // 1. Primero, limpiamos todos los iconos de vida que existían antes para empezar de cero.
        foreach (GameObject icon in activeIcons)
        {
            Destroy(icon);
        }
        activeIcons.Clear();

        // 2. Luego, creamos la cantidad correcta de nuevos iconos.
        //    Si el jugador tiene 3 de vida, este bucle se ejecutará 3 veces.
        for (int i = 0; i < currentHealth; i++)
        {
            // Verificamos que tengamos asignado el prefab y el contenedor para evitar errores.
            if (octopusIconPrefab != null && iconContainer != null)
            {
                // Creamos una nueva instancia del prefab del icono y la hacemos hija del contenedor.
                // Si el contenedor tiene un Layout Group, los iconos se ordenarán solos.
                GameObject newIcon = Instantiate(octopusIconPrefab, iconContainer);
                
                // Añadimos el nuevo icono a nuestra lista para poder borrarlo en la próxima actualización.
                activeIcons.Add(newIcon);
            }
        }
    }
}