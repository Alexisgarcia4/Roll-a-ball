using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement; // Reiniciar la escena

public class PlayerController : MonoBehaviour
{
    public TMP_Text countText;
    public TMP_Text winText;
    public TMP_Text partidaText; 

    public float speed = 10.0f;
    private Rigidbody rb;

    private int count;
    private static int partidaNumero = 1; 

    private float movementX;
    private float movementY;

    public float jumpForce = 3.0f; 
    private bool isGrounded = true; 

    private int totalPickups;

    // Saber si el jugador ha comenzado a moverse
    public bool hasStartedMoving { get; private set; } = false;


    // ------------------------------------------------------ ANTES DE EMPEZAR, INICIAR VARIABLES
    void Start()
    {
        count = 0;  
        SetCountText();  

        rb = GetComponent<Rigidbody>();

        winText.gameObject.SetActive(false); 

        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;
        
        partidaText.text = "Nivel: " + partidaNumero.ToString();
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
        if ( transform.position.y < -10)
        {

            transform.position = new Vector3(0, 0.5f, 0); 
            rb.velocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 


        }
    }
    
   //------------------------------------------------------------ MOVIMIENTO
    private void OnMove(InputValue movementValue)
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


            hasStartedMoving = false;


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
        yield return new WaitForSeconds(3); // Esperar 3 segundos

        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    

    // CARGA SIGUIENTE NIVEL SI HAY Y SI NO VUELVE AL PRIMER NIVEL
    IEnumerator TransitionToNextLevel()
    {
        yield return new WaitForSeconds(3); // Esperar 3 segundos 
        partidaNumero++; 

        // Verificar si existe una escena siguiente
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            // Cargar la siguiente escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // Si no hay más escenas, regresar al primer nivel
            partidaNumero = 1; 
            SceneManager.LoadScene(0); // Cargar la primera escena (índice 0 en Build Settings)
        }
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
