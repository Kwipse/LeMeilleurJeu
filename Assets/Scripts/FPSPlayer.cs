using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace LeMeilleurJeu
{
    public class FPSPlayer : NetworkBehaviour
    {
        //Declarations
        public Camera FPSCameraPrefab;
        Camera fcam;

        translation = transform.position;
        rotation = transform.rotation;
        localTranslation = transform.localPosition;
        localRotation = transform.localRotation;



        void Start()
        {
            //Init FPS Camera
            fcam = Instantiate(FPSCameraPrefab,transform); //Parenting fcam to the FPSPlayer
                                        
        }



        void Update()
        {
            

            MoveFPSPlayer();

        }

        void KeyboardInputs()
        {

            // ZQSD or Arrow keys
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
            {

                translation.x += transform.forward.x * ksensitivity;
                netPosition.z += transform.forward.z * ksensitivity;
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


        void MoveFPSPlayer()
        {
            transform.position = Position.Value;
        }
    }
}