using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RTSCamera : MonoBehaviour
{
    [HideInInspector]
    public bool isZoomActive = true;

    public float vitesseCamera = 2f;
    public float vitesseZoom = 4f;

    public Vector3 InitialCameraPosition;
    public Quaternion InitialCameraRotation; 

    Camera cam;
    Vector3 translation;
    Quaternion rotation;

    Transform camTarget;
    Vector3 previousMousePos; 
    Vector3 pivot;

    float timer;


    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        translation = InitialCameraPosition;
        rotation = InitialCameraRotation;

        pivot = new Vector3(0,0,0);
        camTarget = new GameObject().transform;

        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None; //Debloque la souris
    }

    void Update()
    {
        KeyboardInputs();
        MouseInputs();
        UpdateCamera();
    }

    void KeyboardInputs()
    {
        Vector3 currentTranslation = translation;

        // ZQSD or Arrow keys
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
        {
            translation.x += cam.transform.forward.x * vitesseCamera;
            translation.z += cam.transform.forward.z * vitesseCamera;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            translation.x -= cam.transform.forward.x * vitesseCamera;
            translation.z -= cam.transform.forward.z * vitesseCamera;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            translation.x += cam.transform.right.x * vitesseCamera;
            translation.z += cam.transform.right.z * vitesseCamera;
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            translation.x -= cam.transform.right.x * vitesseCamera;
            translation.z -= cam.transform.right.z * vitesseCamera;
        }

        pivot += translation - currentTranslation;
        camTarget.position += translation - currentTranslation;

    }


    void MouseInputs()
    {
        if (Input.mouseScrollDelta.y != 0) //Scroll
        {
            if (isZoomActive)
                translation += cam.transform.forward * Input.mouseScrollDelta.y * vitesseZoom;
        }

        if (Input.GetMouseButtonDown(2)) //On clic molette down
        {
            Vector3 offset;

            timer = Time.time;
            previousMousePos = Input.mousePosition;

            pivot = GetMouseGroundHit().point;
            offset = pivot - GetCameraCenterHit().point;

            camTarget.position = cam.transform.position;
            camTarget.rotation = cam.transform.rotation;
            camTarget.position += offset;
        }

        if (Input.GetMouseButton(2)) //During clic molette
        {
            //Init
            Vector3 newMousePos;
            Vector3 direction = Vector3.zero;
            float distance;

            //Manage la souris
            newMousePos = Input.mousePosition;
            direction = newMousePos - previousMousePos;
            previousMousePos = newMousePos; 
            
            //Regarde le pivot
            camTarget.Rotate(new Vector3(1, 0, 0), -direction.y);
            camTarget.Rotate(new Vector3(0, 1, 0), direction.x, Space.World); 

            //Limite la rotation verticale
            float rotX = ClampAngle(camTarget.rotation.eulerAngles.x, 0, 80);
            float rotY = camTarget.rotation.eulerAngles.y;
            float rotZ = 0;
            camTarget.rotation = Quaternion.Euler(rotX, rotY, rotZ);
            
            //Distance le pivot
            distance = Vector3.Distance(camTarget.position, pivot);
            camTarget.position = pivot;
            camTarget.Translate(new Vector3(0, 0, -distance));

            //Lerp to camTarget
            float elapsedTime = Time.time - timer ;
            translation = Vector3.Lerp(cam.transform.position, camTarget.position, elapsedTime * vitesseCamera/2f);
            rotation = Quaternion.Lerp(cam.transform.rotation, camTarget.rotation, elapsedTime * vitesseCamera/2f);
        }
    }



    void UpdateCamera()
    {
        cam.transform.position = translation;
        cam.transform.rotation = rotation;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if ((angle >= min) && (angle <= max)) 
            return angle;

        float angleToMin = Mathf.DeltaAngle(min, angle);
        float angleToMax = Mathf.DeltaAngle(angle, max);

        //Debug.Log($"AngleToMin : {angleToMin}, AngleToMax : {angleToMax}");

        if (angleToMin <= angleToMax)
            return min;

        return max;
    }

    public RaycastHit GetMouseGroundHit() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 3000.0f, (1<<8));
        return hit; }

    public RaycastHit GetCameraCenterHit() {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
        Physics.Raycast(ray, out RaycastHit hit, 3000.0f, (1<<8));
        return hit; }


}


