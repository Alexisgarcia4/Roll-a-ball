using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena

public class PlayerController : MonoBehaviour
{
    public TMP_Text countText;
    public TMP_Text winText;
    public TMP_Text partidaText; // Texto para mostrar el n�mero de partida

    public float speed = 10.0f;
    private Rigidbody rb;
    private int count;
    private static int partidaNumero = 1; // Contador de partida

    private float movementX;
    private float movementY;

    // Variable para contar el n�mero total de PickUps en la escena
    private int totalPickups;

    // Start se llama antes del primer frame
    void Start()
    {
        count = 0;  // Inicializar el contador de objetos recogidos
        SetCountText();  // Configurar el texto inicial
        rb = GetComponent<Rigidbody>();
        winText.gameObject.SetActive(false); // Ocultar el texto de victoria al inicio

        // Contar autom�ticamente los pickups al inicio de la escena
        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;
        Debug.Log("Total de PickUps en Start(): " + totalPickups);

        // Mostrar el n�mero de partida en la UI
        partidaText.text = "Nivel: " + partidaNumero.ToString();
        partidaText.gameObject.SetActive(true); // Asegurar que el texto de nivel est� visible
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;

        // Solo ocultar el texto de nivel la primera vez que el jugador se mueva
        if (movementX != 0 || movementY != 0)
        {
            partidaText.gameObject.SetActive(false);
        }
    }

    // FixedUpdate es llamado una vez por frame de f�sica
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    // M�todo que detecta la recolecci�n de objetos (PickUp)
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false); // Desactivar el objeto recolectado
            count++; // Incrementar el contador de objetos recogidos
            SetCountText(); // Actualizar el texto del contador
        }
    }

    // Actualizar el texto que muestra el n�mero de objetos recogidos
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString() + "/" + totalPickups.ToString(); // Mostrar el progreso en la UI

        Debug.Log("Count: " + count + " / TotalPickUps: " + totalPickups);

        // Verificar si el jugador ha recogido todos los objetos antes de pasar de nivel
        if (count >= totalPickups && totalPickups > 0)
        {
            Debug.Log("Todos los objetos recogidos. Transici�n a la siguiente escena.");
            winText.gameObject.SetActive(true);  // Mostrar el texto de victoria
            StartCoroutine(TransitionToNextLevel()); // Transici�n a la siguiente escena
        }
        else
        {
            Debug.Log("A�n faltan objetos por recoger.");
        }
    }

    // Corrutina para esperar antes de cambiar de escena
    IEnumerator TransitionToNextLevel()
    {
        yield return new WaitForSeconds(3); // Esperar 3 segundos antes de la transici�n
        partidaNumero++; // Incrementar el n�mero de partida

        // Verificar si existe una escena siguiente
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            // Cargar la siguiente escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // Si no hay m�s escenas, regresar al primer nivel
            partidaNumero = 1; // Reiniciar el contador de niveles
            SceneManager.LoadScene(0); // Cargar la primera escena (�ndice 0 en Build Settings)
        }
    }
}
