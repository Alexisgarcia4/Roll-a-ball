using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Buttons : MonoBehaviour
{
    // M�todo para cerrar el juego
    public void CerrarJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Solo funciona en el editor
#else
        Application.Quit(); // Cierra el juego en una compilaci�n
#endif
    }

    // M�todo para reiniciar la escena actual
    public void ReiniciarEscenaActual()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Recarga la escena actual
    }

    // M�todo para volver a la primera escena (�ndice 0)
    public void VolverAPrimeraEscena()
    {
        SceneManager.LoadScene(0); // Carga la escena en el �ndice 0
    }
}
