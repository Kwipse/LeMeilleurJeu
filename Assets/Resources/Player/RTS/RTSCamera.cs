using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class RTSCamera : MonoBehaviour
{
    public float vitesseCamera = 2f;
    public float vitesseZoom = 4f;

    public Vector3 InitialCameraPosition;
    public Quaternion InitialCameraRotation; 

    Camera cam;
    Vector3 translation;
    Vector3 localTranslation;
    Quaternion rotation;
    Quaternion localRotation;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        translation = InitialCameraPosition;
        rotation = InitialCameraRotation;

        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None; //Debloque la souris
    }


    // Update is called once per frame
    void Update()
    {
        KeyboardInputs();
        MouseInputs();
        UpdateCamera();
    }

    void KeyboardInputs()
    {
        // ZQSD or Arrow keys
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
        {
            translation.x += transform.forward.x * vitesseCamera;
            translation.z += transform.forward.z * vitesseCamera;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            translation.x -= transform.forward.x * vitesseCamera;
            translation.z -= transform.forward.z * vitesseCamera;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            translation.x += transform.right.x * vitesseCamera;
            translation.z += transform.right.z * vitesseCamera;
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            translation.x -= transform.right.x * vitesseCamera;
            translation.z -= transform.right.z * vitesseCamera;
        }
    }

    void MouseInputs()
    {
        if (Input.mouseScrollDelta.y != 0) //Scroll
        {
            translation += cam.transform.forward * Input.mouseScrollDelta.y * vitesseZoom;
        }
    }

    void UpdateCamera()
    {
        cam.transform.position = translation;
        cam.transform.rotation = rotation;
    }
}


