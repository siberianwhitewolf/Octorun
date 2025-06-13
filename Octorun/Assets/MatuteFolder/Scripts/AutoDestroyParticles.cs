using UnityEngine;

public class AutoDestroyParticles : MonoBehaviour
{
    [SerializeField] private float lifetime = 2.0f;  // Tiempo de vida de las partículas en segundos, editable en el Inspector

    void Start()
    {
        Destroy(gameObject, lifetime);  // Destruye este objeto después de 'lifetime' segundos
    }
}
