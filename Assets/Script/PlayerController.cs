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

    public float jumpForce = 3.0f; // Fuerza de salto
    private bool isGrounded = true; // Variable para verificar si est� en el suelo


    // Variable para contar el n�mero total de PickUps en la escena
    private int totalPickups;

    // Nueva propiedad p�blica para saber si el jugador ha comenzado a moverse
    public bool hasStartedMoving { get; private set; } = false;


    // Start se llama antes del primer frame
    void Start()
    {
        count = 0;  // Inicializar el contador de objetos recogidos
        SetCountText();  // Configurar el texto inicial
        rb = GetComponent<Rigidbody>();
        winText.gameObject.SetActive(false); // Ocultar el texto de victoria al inicio

        // Contar autom�ticamente los pickups al inicio de la escena
        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;
        

        // Mostrar el n�mero de partida en la UI
        partidaText.text = "Nivel: " + partidaNumero.ToString();
        partidaText.gameObject.SetActive(true); // Asegurar que el texto de nivel est� visible

        
    }
    private void Update()
    {
        // Detectar si se presiona la tecla de salto y la bola est� en el suelo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Tecla de salto detectada y en contacto con el suelo");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Aplicar fuerza de salto
            isGrounded = false; // Evitar saltos m�ltiples hasta tocar el suelo de nuevo
        }

        // Comprobar si la bola ha ca�do  z < -10)
        if ( transform.position.y < -10)
        {

            transform.position = new Vector3(0, 0.5f, 0); // Establecer posici�n en (0, 0.5, 0)
            rb.velocity = Vector3.zero; // Detener el movimiento
            rb.angularVelocity = Vector3.zero; // Detener la rotaci�n


        }
    }
    
   
    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;

        // Detectar el primer movimiento del jugador
        if (!hasStartedMoving && (movementX != 0 || movementY != 0))
        {
            hasStartedMoving = true;
        }

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
    // M�todo que detecta que esta tocando suelo Ground
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy the current object
            //Destroy(gameObject);
            // Update the winText to display "You Lose!"
            winText.gameObject.SetActive(true);
            winText.GetComponent<TextMeshProUGUI>().text = "You Lose!";


            hasStartedMoving = false;


            // Iniciar la corrutina para reiniciar la escena despu�s de 3 segundos
            StartCoroutine(RestartSceneAfterDelay());


        }
    }

    // Corrutina para reiniciar la escena despu�s de mostrar el mensaje de derrota
    IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(3); // Esperar 3 segundos

        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Actualizar el texto que muestra el n�mero de objetos recogidos
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString() ; // Mostrar el progreso en la UI

        Debug.Log("Count: " + count + " / TotalPickUps: " + totalPickups);

        // Verificar si el jugador ha recogido todos los objetos antes de pasar de nivel
        if (count >= totalPickups && totalPickups > 0)
        {
            Debug.Log("Todos los objetos recogidos. Transici�n a la siguiente escena.");
            winText.gameObject.SetActive(true);  // Mostrar el texto de victoria
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
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
