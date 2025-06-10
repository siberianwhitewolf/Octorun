using UnityEngine;
using TMPro; // Usaremos TextMeshPro, que es el estándar actual en Unity.

public class InkDisplayUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("Arrastra aquí el componente de texto que mostrará la cantidad de tinta.")]
    public TextMeshProUGUI inkText;

    // Al activarse, se suscribe al evento del InkManager.
    private void OnEnable()
    {
        InkManager.OnInkChanged += UpdateInkText;
    }

    // Al desactivarse, se desuscribe para evitar errores.
    private void OnDisable()
    {
        InkManager.OnInkChanged -= UpdateInkText;
    }

    /// <summary>
    /// Esta función se llama automáticamente cuando el InkManager emite el evento OnInkChanged.
    /// </summary>
    /// <param name="current">La tinta actual.</param>
    /// <param name="max">La tinta máxima.</param>
    private void UpdateInkText(float current, float max)
    {
        if (inkText != null)
        {
            // Usamos el formato "F0" para mostrar los floats como si fueran enteros (sin decimales).
            inkText.text = $"TINTA: {current:F0}/{max:F0}";
        }
    }
}