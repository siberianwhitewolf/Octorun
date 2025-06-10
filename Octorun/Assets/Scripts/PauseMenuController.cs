using UnityEngine;
using UnityEngine.SceneManagement; // Opcional, por si añades botones de menú

public class PauseMenuController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el panel principal de tu menú de pausa.")]
    public GameObject pauseMenuPanel;

    // Variable privada para saber si el juego está pausado o no.
    private bool isPaused = false;

    void Start()
    {
        // Nos aseguramos de que al empezar el juego, el menú esté oculto y el tiempo corra normalmente.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        Time.timeScale = 1f; // Asegura que el tiempo no empiece congelado.
    }

    void Update()
    {
        // Si el jugador presiona la tecla Escape...
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ...llamamos a nuestra función para cambiar el estado de la pausa.
            TogglePause();
        }
    }

    /// <summary>
    /// Esta función pública cambia el estado del juego entre pausado y activo.
    /// La puedes asignar al evento OnClick de un botón.
    /// </summary>
    public void TogglePause()
    {
        // Invertimos el estado actual. Si estaba en true, pasa a false, y viceversa.
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    // EN TU SCRIPT PauseMenuController.cs
    
    private void PauseGame()
    {
        // Activamos el objeto del panel del menú para que sea visible.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    
        // Congelamos el tiempo del juego.
        Time.timeScale = 0f;
    
        // --- LÓGICA DEL CURSOR (MUY IMPORTANTE) ---
        // Desbloqueamos el cursor para que se pueda mover libremente por el menú.
        Cursor.lockState = CursorLockMode.None;
        // Hacemos que el cursor del sistema operativo sea visible.
        Cursor.visible = true;
        
        Debug.Log("Juego Pausado. Cursor desbloqueado y visible.");
    }
    
    private void ResumeGame()
    {
        // Desactivamos el panel del menú para ocultarlo.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    
        // Reanudamos el tiempo del juego.
        Time.timeScale = 1f;
        
        // --- LÓGICA DEL CURSOR (MUY IMPORTANTE) ---
        // Volvemos a bloquear y ocultar el cursor para el gameplay.
        // Si tu juego no necesita bloquear el cursor, puedes comentar estas dos líneas.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Juego Reanudado");
    }
}