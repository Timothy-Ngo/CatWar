using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float zoomSpeed = 5f;
    public float maxSize = 10f;

    public float minSize = 4f;

    // keep the camera within these bounds
    public float minX = 25f;
    public float maxX = 265f;

    public float minY = 20f;
    public float maxY = 130f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().orthographicSize = maxSize;
        transform.position = new Vector3(minX, (minY + maxY) / 2, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += (Vector3.left * (movementSpeed * Time.deltaTime));
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += (Vector3.right * (movementSpeed * Time.deltaTime));
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += (Vector3.up * (movementSpeed * Time.deltaTime));
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += (Vector3.down * (movementSpeed * Time.deltaTime));
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
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
