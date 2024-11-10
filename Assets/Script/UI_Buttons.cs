using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Buttons : MonoBehaviour
{
    // Método para cerrar el juego
    public void CerrarJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Solo funciona en el editor
#else
        Application.Quit(); // Cierra el juego en una compilación
#endif
    }

    // Método para reiniciar la escena actual
    public void ReiniciarEscenaActual()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Recarga la escena actual
    }

    // Método para volver a la primera escena (índice 0)
    public void VolverAPrimeraEscena()
    {
        SceneManager.LoadScene(0); // Carga la escena en el índice 0
    }
}
