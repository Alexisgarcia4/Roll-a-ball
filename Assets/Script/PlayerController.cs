using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour


{
    public TMP_Text countText;
    public TMP_Text winText;


    public float speed = 10.0f;
    private Rigidbody rb;
    private int count;

    private float movementX;
    private float movementY;


    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        SetCountText();
        rb = GetComponent<Rigidbody>();
        winText.gameObject.SetActive(false);
        
    }
    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>(); 

        movementX=movementVector.x;
        movementY=movementVector.y; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement= new Vector3(movementX,0.0f,movementY);
        rb.AddForce(movement*speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 5) { 
            winText.gameObject.SetActive(true);
        }
    }
}
