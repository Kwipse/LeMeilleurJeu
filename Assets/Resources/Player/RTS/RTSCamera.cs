using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class RTSCamera : MonoBehaviour
{
    // Init Prefabs
    public Vector3 InitialCameraPosition;
    public Quaternion InitialCameraRotation; 
    Camera cam;

    //Init Sensibilite
    public float ksensitivity = 2f; //Sensibilit� Clavier
    public float zsensitivity = 4f; //Sensibilit� Zoom


    Vector3 translation;
    Vector3 localTranslation;
    Quaternion rotation;
    Quaternion localRotation;


    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        
        translation = InitialCameraPosition;
        rotation = InitialCameraRotation;

        //cam.transform.parent = transform;
        //cam.transform.position = InitialCameraPosition;
        //cam.transform.rotation = InitialCameraRotation;

        Cursor.lockState = CursorLockMode.Confined;

        //translation = cam.transform.position;
        //localTranslation = cam.transform.localPosition;
        //rotation = cam.transform.rotation;
        //localRotation = cam.transform.localRotation;
    }

    void UpdateCamera()
    {
        cam.transform.position = translation;
        cam.transform.rotation = rotation;
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
            translation.x += transform.forward.x * ksensitivity;
            translation.z += transform.forward.z * ksensitivity;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            translation.x -= transform.forward.x * ksensitivity;
            translation.z -= transform.forward.z * ksensitivity;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            translation.x += transform.right.x * ksensitivity;
            translation.z += transform.right.z * ksensitivity;
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            translation.x -= transform.right.x * ksensitivity;
            translation.z -= transform.right.z * ksensitivity;
        }

        //// A and E to rotate camera
        //if (Input.GetKey(KeyCode.A))
        //{
        //    rotation *= Quaternion.Euler(Vector3.up * -ksensitivity );
        //}
        //if (Input.GetKey(KeyCode.E))
        //{
        //    rotation *= Quaternion.Euler(Vector3.up * ksensitivity);
        //}
    }

    void MouseInputs()
    {
        if (Input.mouseScrollDelta.y != 0) //Scroll
        {
            translation += cam.transform.forward * Input.mouseScrollDelta.y * zsensitivity;
        }
    }

}


