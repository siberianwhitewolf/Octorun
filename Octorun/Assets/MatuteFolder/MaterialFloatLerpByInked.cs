//TPFinal - * Nombre y apellido del alumno *
using System.Collections.Generic;
using UnityEngine;

public class MaterialFloatLerpByInked : MonoBehaviour
{
    [Header("Nombre del parámetro Float (ej: _Float)")]
    public string floatPropertyName = "_Float";

    [Header("Velocidad de transición")]
    public float lerpSpeed = 1f;

    [Header("Materiales a controlar")]
    public List<Material> materialsToControl = new List<Material>();

    [Header("Control desde Inspector")]
    public bool IsInked = false;

    private float targetValue = 1f;
    private bool isLerping = false;
    private bool previousIsInked = false;

    private const float minValue = 0.33f;
    private const float maxValue = 1f;

    void Update()
    {
        // Cambió el valor del booleano desde el inspector y todos los materiales están en un estado válido
        if (IsInked != previousIsInked && !isLerping && AllFloatsAtSameValue())
        {
            targetValue = IsInked ? minValue : maxValue;
            isLerping = true;
            previousIsInked = IsInked;
        }

        if (isLerping)
        {
            bool allReached = true;

            foreach (Material mat in materialsToControl)
            {
                if (mat == null) continue;

                float current = mat.GetFloat(floatPropertyName);
                float newValue = Mathf.MoveTowards(current, targetValue, lerpSpeed * Time.deltaTime);
                mat.SetFloat(floatPropertyName, newValue);

                if (!Mathf.Approximately(newValue, targetValue))
                    allReached = false;
            }

            if (allReached)
                isLerping = false;
        }
    }

    private bool AllFloatsAtSameValue()
    {
        if (materialsToControl.Count == 0) return false;

        float reference = materialsToControl[0].GetFloat(floatPropertyName);
        foreach (Material mat in materialsToControl)
        {
            if (mat == null) continue;

            float value = mat.GetFloat(floatPropertyName);
            if (!Mathf.Approximately(value, reference))
                return false;
        }

        return Mathf.Approximately(reference, minValue) || Mathf.Approximately(reference, maxValue);
    }
}
