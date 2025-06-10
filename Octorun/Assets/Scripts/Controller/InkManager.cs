using System;
using UnityEngine;

public class InkManager : MonoBehaviour
{
    [Header("Configuración de Tinta")]
    public float maxInk = 100f;
    [Tooltip("Tinta que se regenera por segundo.")]
    public float inkRechargeRate = 5f;
    [Tooltip("Segundos a esperar después de usar tinta antes de que empiece a regenerarse.")]
    public float inkRechargeDelay = 2f;

    private float currentInk;
    private float timeSinceLastUse = 0f;

    // El evento ahora envía floats para mayor precisión
    public static event Action<float, float> OnInkChanged;
    public float CurrentInk => currentInk;

    private void Awake()
    {
        currentInk = maxInk;
    }

    private void Start()
    {
        OnInkChanged?.Invoke(currentInk, maxInk);
    }

    private void Update()
    {
        // Incrementamos el temporizador desde el último uso.
        timeSinceLastUse += Time.deltaTime;

        // Si ha pasado suficiente tiempo desde el último uso, regeneramos la tinta.
        if (timeSinceLastUse >= inkRechargeDelay)
        {
            // Solo regeneramos si no estamos ya al máximo.
            if (currentInk < maxInk)
            {
                currentInk += inkRechargeRate * Time.deltaTime;
                currentInk = Mathf.Min(currentInk, maxInk); // Clamp para no sobrepasar el máximo
                OnInkChanged?.Invoke(currentInk, maxInk); // Anunciamos el cambio
            }
        }
    }

    public bool UseInk(float amountToUse)
    {
        // Reseteamos el temporizador cada vez que intentamos usar tinta.
        // Esto detiene la regeneración mientras la habilidad está activa.
        timeSinceLastUse = 0f;

        if (currentInk >= amountToUse)
        {
            currentInk -= amountToUse;
            OnInkChanged?.Invoke(currentInk, maxInk);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RestoreInk(float amountToRestore)
    {
        currentInk = Mathf.Min(currentInk + amountToRestore, maxInk);
        OnInkChanged?.Invoke(currentInk, maxInk);
    }
}