using UnityEngine;

/// TPFinal – * Nombre y apellido del alumno *
/// Pon este componente al **Objeto A** (debe tener un Collider con *Is Trigger*).
/// ─ Al arrancar la escena **apaga** los scripts listados en `scriptsToToggle`.
/// ─ Si entra en el trigger un objeto cuya **Layer** sea “Player”, los enciende.
/// ─ Cuando el objeto de la capa Player sale, los vuelve a apagar.
///   (Este componente nunca se deshabilita a sí mismo).

[RequireComponent(typeof(Collider))]
public class SelfScriptToggleOnPlayer : MonoBehaviour
{
    [Tooltip("Componentes de este mismo objeto que quieres activar/desactivar")]
    [SerializeField] private Behaviour[] scriptsToToggle;

    private int playerLayer;          // Capa “Player” convertida a entero

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");

        // Asegura que el Collider sea trigger
        Collider col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        // Garantiza que los scripts objetivo arranquen apagados
        ToggleScripts(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
            ToggleScripts(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
            ToggleScripts(false);
    }

    // Encender / apagar los componentes indicados sin afectarse a sí mismo
    private void ToggleScripts(bool state)
    {
        foreach (var s in scriptsToToggle)
            if (s && s != this)
                s.enabled = state;
    }
}
