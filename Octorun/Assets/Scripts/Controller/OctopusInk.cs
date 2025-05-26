using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusInk : MonoBehaviour
{
    [Header("Ink Settings")]
    public float inkMax = 100f;
    public float inkCostPerSecond = 10f;
    public float inkRechargeRate = 5f;
    public KeyCode fireKey = KeyCode.Mouse0;

    [Header("Spray Settings")]
    public float sprayRadius = 4f;
    public float sprayAngle = 30f;

    public float currentInk;
    public ParticleSystem inkParticle;
    private ParticleSystem[] allInkParticles;
    private bool isFiring = false;

    void Start()
    {
        allInkParticles = inkParticle.GetComponentsInChildren<ParticleSystem>();
        currentInk = inkMax;
    }

    void Update()
    {
        bool shootPressed = Input.GetKey(fireKey);

        if (shootPressed && currentInk > 0f)
        {
            if (!isFiring)
            {
                foreach (var ps in allInkParticles)
                    ps.Play();

                isFiring = true;
            }

            currentInk -= inkCostPerSecond * Time.deltaTime;
            currentInk = Mathf.Max(0f, currentInk);
            if (currentInk == 0)  
            {
                foreach (var ps in allInkParticles)
                    ps.Stop();
            }
        }
        else
        {
            if (isFiring)
            {
                foreach (var ps in allInkParticles)
                    ps.Stop();

                isFiring = false;
            }

            currentInk += inkRechargeRate * Time.deltaTime;
            currentInk = Mathf.Min(inkMax, currentInk);
        }
    }

    public float GetInkLevelNormalized() => currentInk / inkMax;
}
