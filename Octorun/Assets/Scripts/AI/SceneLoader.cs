using UnityEngine;
using UnityEngine.SceneManagement;

// Usamos una directiva de compilación para incluir el editor de Unity solo cuando estamos en el editor.
// Esto evita errores al compilar el juego final.
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Carga una escena por su nombre. Esta función es pública para poder ser llamada por UnityEvents.
    /// </summary>
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("El nombre de la escena no puede estar vacío.");
            return;
        }
        Debug.Log($"Cargando escena: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Cierra la aplicación si es un juego compilado, o detiene el modo de juego si está en el editor de Unity.
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Intentando salir del juego...");

        // Usamos directivas de compilación para que el código se comporte diferente
        // dependiendo de si estamos en el editor o en el juego final.

#if UNITY_EDITOR
        // Si estamos ejecutando el juego dentro del editor de Unity, detenemos la reproducción.
        EditorApplication.isPlaying = false;
#else
            // Si estamos en un juego compilado (ej. un .exe), cerramos la aplicación.
            Application.Quit();
#endif
    }
}