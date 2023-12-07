using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float zoomSpeed = 5f;
    public float maxSize = 10f;

    public float minSize = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += (Vector3.left * (movementSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += (Vector3.right * (movementSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += (Vector3.up * (movementSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += (Vector3.down * (movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.F))
        {
            ZoomOut();
        }
        else if (Input.GetKey(KeyCode.R))
        {
            ZoomIn();
        }
    }


    public void ZoomOut()
    {
        Camera mCamera = GetComponent<Camera>();
        if (mCamera.orthographicSize <= maxSize)
        {
            mCamera.orthographicSize += Time.deltaTime * zoomSpeed;
        }
    }

    public void ZoomIn()
    {
        Camera mCamera = GetComponent<Camera>();
        if (mCamera.orthographicSize >= minSize)
        {
            mCamera.orthographicSize -= Time.deltaTime * zoomSpeed;
        }
    }
}
