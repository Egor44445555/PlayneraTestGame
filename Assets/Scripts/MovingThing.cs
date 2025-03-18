using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingThing : MonoBehaviour
{
    Rigidbody2D rb;
    Vector3 offset;
    float zPosition;
    bool isDragging = false;
    bool isFalling = false;
    bool onFloor = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            // First touch
            Touch touch = Input.GetTouch(0);

            // Touch position to world coordinates
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = zPosition;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Touch check
                    if (IsTouchingObject(touchPosition))
                    {
                        // Save offset
                        offset = transform.position - touchPosition;
                        isDragging = true;

                        // Making object static
                        rb.bodyType = RigidbodyType2D.Kinematic;
                        isFalling = false;
                    }
                    break;

                case TouchPhase.Moved:
                    // Move object
                    if (isDragging)
                    {
                        transform.position = touchPosition + offset;
                        CameraTouchMove.main.cameraMove = false;
                    }
                    break;

                case TouchPhase.Ended:
                    // Remove finger from the screen
                    if (isDragging)
                    {
                        isDragging = false;                        

                        if (onFloor)
                        {
                            isFalling = false;
                            rb.bodyType = RigidbodyType2D.Kinematic;
                            rb.velocity = Vector2.zero;
                        }
                        else
                        {
                            isFalling = true;
                            rb.bodyType = RigidbodyType2D.Dynamic;
                        }

                        CameraTouchMove.main.cameraMove = true;
                    }
                    break;
            }
        }

        if (isFalling)
        {
            // Acceleration when falling
            rb.AddForce(Vector2.down * 1f, ForceMode2D.Force);
            rb.freezeRotation = false;
        }
        else
        {
            rb.freezeRotation = true;
        }
    }
    
    private bool IsTouchingObject(Vector3 touchPosition)
    {
        // Touch check on object
        Collider2D hit = Physics2D.OverlapPoint(touchPosition);
        return hit != null && hit.gameObject == gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Checking whether an object is included in the area with the tag "DragArea"
        if (other.CompareTag("DragArea"))
        {
            onFloor = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            isFalling = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Checking if an object is out of the area with a tag "DragArea"
        if (other.CompareTag("DragArea")) 
        {
            onFloor = false;

            if (!isDragging)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isFalling = true;
            }
        }
    }
}
