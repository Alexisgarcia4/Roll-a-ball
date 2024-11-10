using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Specialized;


public class PlayerController : MonoBehaviour
{
    public TMP_Text countText;
    public TMP_Text winText;
    public TMP_Text partidaText;

    public float speed = 10.0f;
    private Rigidbody rb;

    private int count;
   

    private float movementX;
    private float movementY;

    public float jumpForce = 3.0f;
    private bool isGrounded = true;

    private int totalPickups;

    // Saber si el jugador ha comenzado a moverse
    public bool hasStartedMoving { get; private set; } = false;

    public GameObject cerrarJuegoButton; // Referencia al botón de Cerrar Juego
    public GameObject reinicioMismaEscenaButton; // Referencia al botón de reinicio escena actual
    public GameObject volverInicioButton; // Referencia al botón de Volver al Inicio

    private bool atrapado = false;


    // ------------------------------------------------------ ANTES DE EMPEZAR, INICIAR VARIABLES
    void Start()
    {
        count = 0;
        SetCountText();

        rb = GetComponent<Rigidbody>();

        winText.gameObject.SetActive(false);

        // Verificar y ocultar los botones solo si están asignados
        if (cerrarJuegoButton != null)
            cerrarJuegoButton.SetActive(false);
        if (reinicioMismaEscenaButton != null)
            reinicioMismaEscenaButton.SetActive(false);
        if (volverInicioButton != null)
            volverInicioButton.SetActive(false);

        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;

        // Ajustar el texto de nivel según el índice de la escena
        int nivelActual = SceneManager.GetActiveScene().buildIndex + 1;
        partidaText.text = "Nivel: " + nivelActual.ToString();
        partidaText.gameObject.SetActive(true);


    }

    //------------------------------------- SALTO
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Evitar saltos múltiples 
        }

        // Comprobar si la bola ha caído
        if (transform.position.y < -10)
        {

            transform.position = new Vector3(0, 0.5f, 0);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;


        }
    }

    //------------------------------------------------------------ MOVIMIENTO
    private void OnMove(InputValue movementValue)
    {

        if (!atrapado)
        {
            Vector2 movementVector = movementValue.Get<Vector2>();

            movementX = movementVector.x;
            movementY = movementVector.y;

            // Detectar el primer movimiento 
            if (!hasStartedMoving && (movementX != 0 || movementY != 0))
            {
                hasStartedMoving = true;
            }

            // Ocultar el texto de nivel 
            if (movementX != 0 || movementY != 0)
            {
                partidaText.gameObject.SetActive(false);
            }

        }
        


    }

    // ---------------------------------------- FixedUpdate es llamado una vez por frame de física
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

    }




    //----------------------------------- DETECTA PASO SIN QUE COLISION FISICA, ISTRIGGER ACTIVADO
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }
    // ------------------------------------------------- DETECTA COLISIONES FISICAS, AMBOS OBJETOS COLLIDER
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy the current object
            //Destroy(gameObject);
            // Update the winText to display "You Lose!"
            winText.gameObject.SetActive(true);
            winText.GetComponent<TextMeshProUGUI>().text = "You Lose!";
            atrapado = true;

            hasStartedMoving = false;

            rb.velocity = Vector3.zero;
            rb.isKinematic = true;


            // Iniciar la corrutina para reiniciar la escena después de 3 segundos
            StartCoroutine(RestartSceneAfterDelay());


        }
    }

    // ------------------------------------------------- DETECTA SI NO HAY COLISIONES FISICAS, AMBOS OBJETOS COLLIDER
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }


    }

    // CARGA LA MISMA ESCENA EN LA QUE ESTA
    IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(1); // Esperar 3 segundos


        // Mostrar el mensaje de derrota y los botones de Cerrar Juego y Reiniciar Escena
        winText.gameObject.SetActive(true);
        winText.GetComponent<TextMeshProUGUI>().text = "You Lose!";


        if (cerrarJuegoButton != null)
            cerrarJuegoButton.SetActive(true);

        if (reinicioMismaEscenaButton != null)
            reinicioMismaEscenaButton.SetActive(true);
    }



    // CARGA SIGUIENTE NIVEL SI HAY Y SI NO VUELVE AL PRIMER NIVEL
    IEnumerator TransitionToNextLevel()
    {
        yield return new WaitForSeconds(3); // Esperar 3 segundos
        
        /**
        // Si estamos en la primera escena (índice 0)
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // Cargar la segunda escena (índice 1)
            SceneManager.LoadScene(1);
        }
        // Si estamos en la segunda escena (índice 1)
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {**/
            // Mostrar el mensaje de victoria y los botones de Cerrar Juego y Volver al Inicio
            winText.gameObject.SetActive(true);
            winText.GetComponent<TextMeshProUGUI>().text = "You Win!";

            if (cerrarJuegoButton != null)
                cerrarJuegoButton.SetActive(true);

            if (volverInicioButton != null)
                volverInicioButton.SetActive(true);
       // }
    }

    // ------------------------------------------------ ACTUALIZAR TEXTO  Y ACTIVA SIGUIENTE NIVEL
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();




        if (count >= totalPickups && totalPickups > 0)
        {

            winText.gameObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
            StartCoroutine(TransitionToNextLevel()); // Transición a la siguiente escena
        }

    }
}