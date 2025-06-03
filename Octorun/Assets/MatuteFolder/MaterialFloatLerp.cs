//TPFinal - * Nombre y apellido del alumno *
using System.Collections.Generic;
using UnityEngine;

public class MaterialFloatLerp : MonoBehaviour
{
    [Header("Nombre del parámetro Float (ej: _Float)")]
    public string floatPropertyName = "_Float";

    [Header("Velocidad de transición")]
    public float lerpSpeed = 1f;

    [Header("Materiales a controlar")]
    public List<Material> materialsToControl = new List<Material>();

    private float targetValue = 1f;
    private bool isLerping = false;
    public bool triggered;

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Q) && !isLerping && AllFloatsAtSameValue()) || triggered)
        {
            // Alternar target: si estamos en 0, ir a 1. Si estamos en 1, ir a 0.
            targetValue = Mathf.Approximately(materialsToControl[0].GetFloat(floatPropertyName), 1f) ? 0f : 1f;
            isLerping = true;
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
            {
                triggered = false;
                isLerping = false;
            }
                
        }
    }

    private bool AllFloatsAtSameValue()
    {
        if (materialsToControl.Count == 0) return false;

        float reference = materialsToControl[0].GetFloat(floatPropertyName);
        foreach (Material mat in materialsToControl)
        {
            if (mat == null || !Mathf.Approximately(mat.GetFloat(floatPropertyName), reference))
                return false;
        }
        return Mathf.Approximately(reference, 0f) || Mathf.Approximately(reference, 1f);
    }
}
