using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace LeMeilleurJeu
{
    public class FPSCamera : NetworkBehaviour
    {
        //Init 
        GameObject player;
        Camera cam ;

        float sensitivity = 2f;


        Vector2 rotation = Vector2.zero;
        const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
        const string yAxis = "Mouse Y";

        void Awake()
        {
            
            cam = GetComponent<Camera>(); //Get FPSCamera
            player = transform.parent.gameObject; // Get FPSPlayer


			Cursor.lockState = CursorLockMode.Locked; //Bloque la souris au centre
			Cursor.visible = false; // Fait disparaitre le curseur de la souris

        }



        void OnDisable()
        {
            if (IsOwner)
            {
                Cursor.lockState = CursorLockMode.None; // Débloque la souris
                Cursor.visible = true; // Rend visible le curseur
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (IsOwner)
            {
                rotation.x += Input.GetAxis(xAxis) * sensitivity;
                rotation.y += Input.GetAxis(yAxis) * sensitivity;
                rotation.y = Mathf.Clamp(rotation.y, -90, 90);
                var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
                var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
               

                cam.transform.localRotation = yQuat;
                player.transform.rotation = xQuat;
            }
        }


    }
}
