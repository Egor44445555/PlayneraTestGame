using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraTouchMove : MonoBehaviour
{
    public static CameraTouchMove main;
    public GameObject topLeftPoint;
    public GameObject bottomRightPoint;
    public float moveSpeed = 0.2f;

    [HideInInspector] public bool cameraMove = true;

    Camera cam;
    Vector2 touchStartPos;
    Vector3 cameraStartPos;
    Vector2 minBounds;
    Vector2 maxBounds;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        // Setting camera bounds
        minBounds.x = topLeftPoint.transform.position.x;
        maxBounds.x = bottomRightPoint.transform.position.x;
        minBounds.y = bottomRightPoint.transform.position.y;        
        maxBounds.y = topLeftPoint.transform.position.y;
        cam = Camera.main;
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -10);
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // First point of finger contact
                touchStartPos = touch.position;
                cameraStartPos = Camera.main.transform.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // Finger movement on the screen
                Vector2 touchDelta = touch.position - touchStartPos;
                Vector3 newPos = cameraStartPos - new Vector3(touchDelta.x * moveSpeed, touchDelta.y * moveSpeed, 0);

                newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
                newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
                newPos.z = -100;

                if (cameraMove)
                {
                    cam.transform.position = newPos;
                }
            }
        }

        LimitCameraPosition();
    }

    void LimitCameraPosition()
    {
        // Camera boundary checking
        Vector3 pos = cam.transform.position;

        pos.x = Mathf.Clamp(pos.x, minBounds.x + GetHorizontalLimit(), maxBounds.x - GetHorizontalLimit());
        pos.y = Mathf.Clamp(pos.y, minBounds.y + GetVerticalLimit(), maxBounds.y - GetVerticalLimit());
        pos.z = -100;

        cam.transform.position = pos;
    }

    float GetHorizontalLimit()
    {
        return cam.orthographicSize * cam.aspect;
    }

    float GetVerticalLimit()
    {
        return cam.orthographicSize;
    }
}
