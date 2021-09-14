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

        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }
        [Range(0.1f, 9f)] [SerializeField] float moveSpeed = 2f;



        //Declarations
        public Camera FPSCameraPrefab;
        Camera fcam;

        Vector3 translation;
        Vector3 localTranslation;
        Quaternion rotation;
        Quaternion localRotation;

       


        void Start()
        {
            //Init FPS Camera
            fcam = Instantiate(FPSCameraPrefab,transform); //Parenting fcam to the FPSPlayer

            //Init Transform Variables
            translation = transform.position;
            rotation = transform.rotation;
            localTranslation = transform.localPosition;
            localRotation = transform.localRotation;
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

                translation.x += transform.forward.x * moveSpeed;
                translation.z += transform.forward.z * moveSpeed;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                translation.x -= transform.forward.x * moveSpeed;
                translation.z -= transform.forward.z * moveSpeed;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                translation.x += transform.right.x * moveSpeed;
                translation.z += transform.right.z * moveSpeed;
            }
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
            {
                translation.x -= transform.right.x * moveSpeed;
                translation.z -= transform.right.z * moveSpeed;

            }

        }


        void MoveFPSPlayer()
        {
            transform.position = translation;
        }
    }
}