using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;


using UnityEngine;


    public class FPSCamera : MonoBehaviour
    {
		
		SpawnManager SM;
		
        //Init Public
		public float sensitivity ;
		public Vector3 InitialCameraPosition ;
		
		//Init Private
        Vector2 rotation = Vector2.zero;
        const string xAxis = "Mouse X"; 
        const string yAxis = "Mouse Y";
		Camera cam;

		
		void Awake()
		{

				cam = GetComponent<Camera>();

				cam.transform.parent = transform;
				cam.transform.position = InitialCameraPosition;
				Debug.Log("Current Camera : " + cam);
				
				
				Cursor.lockState = CursorLockMode.Locked; //Bloque la souris au centre
				Cursor.visible = false; // Fait disparaitre le curseur de la souris
			
		}
		


        void OnDisable()
        {

                Cursor.lockState = CursorLockMode.None; // D�bloque la souris
                Cursor.visible = true; // Rend visible le curseur
				
        }


        void Update()
        {

                rotation.x += Input.GetAxis(xAxis) * sensitivity;
                rotation.y += Input.GetAxis(yAxis) * sensitivity;
                rotation.y = Mathf.Clamp(rotation.y, -90, 90);
                var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
                var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
               

                cam.transform.localRotation = yQuat;
                cam.transform.rotation = xQuat;

        }


    }

