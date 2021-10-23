using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;


    public class FPSCamera : NetworkBehaviour
    {
        //Init Public
		public Camera camPrefab ;
		public float sensitivity ;
		public Vector3 InitialCameraPosition ;
		
		//Init Private
        Vector2 rotation = Vector2.zero;
        const string xAxis = "Mouse X"; 
        const string yAxis = "Mouse Y";
		Camera cam;

		
		public override void NetworkStart()
		{
			if (!IsOwner) {enabled=false;}
			else
			{
				//Spawn camera
				cam = Instantiate(camPrefab);
				cam.transform.parent = transform;
				cam.transform.position = InitialCameraPosition;
				Debug.Log("Current Camera : " + cam);
				
				
				Cursor.lockState = CursorLockMode.Locked; //Bloque la souris au centre
				Cursor.visible = false; // Fait disparaitre le curseur de la souris
			}
		}
		


        void OnDisable()
        {
            if (IsOwner)
            {
                Cursor.lockState = CursorLockMode.None; // Débloque la souris
                Cursor.visible = true; // Rend visible le curseur
            }
        }


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
                transform.rotation = xQuat;
            }
        }


    }

