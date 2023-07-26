using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace LeMeilleurJeu
{
    public class RTSCamera : NetworkBehaviour
    {
		// Init Prefabs
		public Camera camPrefab;
		public Vector3 InitialCameraPosition;
		Camera cam;
		
        //Init Sensibilit�s
        //float msensitivity = 2f; //Sensibilit� Souris
        float ksensitivity = 2f; //Sensibilit� Clavier
        float zsensitivity = 2f; //Sensibilit� Zoom


        Vector3 translation;
        Vector3 localTranslation;
        Quaternion rotation;
        Quaternion localRotation;


		

        public override void OnNetworkSpawn()
        {
			if (!IsOwner) {enabled=false;}
			else
			{
				//Spawn Camera
				cam = Instantiate(camPrefab);
				cam.transform.parent = transform;
				cam.transform.position = InitialCameraPosition;
				Debug.Log("Current Camera : " + cam);
								
				//Lock cursor in window
				Cursor.lockState = CursorLockMode.Confined;
				
				//Init things
				translation = cam.transform.position;
				rotation = cam.transform.rotation;
				
			}
        }

        void OnDisable()
        {
            if (IsOwner) {Cursor.lockState = CursorLockMode.None;} //D�bloque la souris
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

            // A and E to rotate camera
            if (Input.GetKey(KeyCode.A))
            {
                rotation *= Quaternion.Euler(Vector3.up * ksensitivity * 3);
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotation *= Quaternion.Euler(Vector3.up * -ksensitivity * 3);
            }

        }

        void MouseInputs()
        {


            //Scroll
            if (Input.mouseScrollDelta.y != 0)
            {
                translation += transform.forward * Input.mouseScrollDelta.y * zsensitivity;
            }


        }


        void UpdateCamera()
        {

            //Apply Translations
            cam.transform.position = translation;


            //Apply Rotations
            cam.transform.rotation = rotation;


        }


    }
}

