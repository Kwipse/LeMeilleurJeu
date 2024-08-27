using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FPSCamera : NetworkBehaviour
{
    public float sensitivity ;
    public Vector3 InitialCameraPosition ;

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X"; 
    const string yAxis = "Mouse Y";

    Camera cam;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) {enabled = false;}
    }

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        cam.transform.SetParent(transform); //Parent player to camera
        //cam.transform.position = InitialCameraPosition;

        Cursor.lockState = CursorLockMode.Locked; //Bloque la souris au centre
        Cursor.visible = false; // Fait disparaitre le curseur de la souris
    }


    void Update()
    {
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -90, 90);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        //Axe horizontal - FPSPlayer rotationne
        transform.localRotation = xQuat;

        //Axe vertical - FPSPlayer ne rotationne pas
        cam.transform.localRotation = yQuat;
    }

    public override void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None; // Débloque la souris
        Cursor.visible = true; // Rend visible le curseur

        base.OnDestroy();
    }
}

