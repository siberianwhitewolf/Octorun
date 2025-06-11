// TPFinal - * Nombre y apellido del alumno *
using UnityEngine;

/// <summary>
/// Baja "_Outline_thickness" de 1 → 0 en <descentDuration> seg.
/// Al llegar a 0 lo mantiene <holdDuration> seg, luego salta a 1 y repite.
/// Funciona incluso desde un GameObject vacío (usa el asset del material).
/// </summary>
public class OutlineThicknessLooper : MonoBehaviour
{
    [Header("Material y parámetro")]
    [SerializeField] private Material targetMaterial;
    [SerializeField] private string floatName = "_Outline_thickness";

    [Header("Tiempos (segundos)")]
    [SerializeField] private float descentDuration = 1f;   // 1 → 0
    [SerializeField] private float holdDuration = 1f;   // tiempo en 0

    const float maxValue = 1f;
    const float minValue = 0f;

    float elapsed;           // para la bajada
    float holdTimer;         // para la pausa en 0
    bool holding;           // ¿está en pausa?

    void Reset()
    {
        if (targetMaterial == null && TryGetComponent(out Renderer r))
            targetMaterial = r.sharedMaterial;
    }

    void Update()
    {
        if (targetMaterial == null) return;

        if (!holding)                                        // fase de descenso
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / descentDuration);
            targetMaterial.SetFloat(floatName, Mathf.Lerp(maxValue, minValue, t));

            if (elapsed >= descentDuration)                 // llegó a 0
            {
                holding = true;
                holdTimer = holdDuration;
                targetMaterial.SetFloat(floatName, minValue);
            }
        }
        else                                                // fase de pausa en 0
        {
            holdTimer -= Time.deltaTime;
            if (holdTimer <= 0f)
            {
                targetMaterial.SetFloat(floatName, maxValue); // reset a 1
                elapsed = 0f;
                holding = false;
            }
        }
    }
}
