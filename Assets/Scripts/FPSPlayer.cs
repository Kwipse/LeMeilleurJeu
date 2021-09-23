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
        Camera cam;

        Vector3 translation;
        Vector3 localTranslation;
        Quaternion rotation;
        Quaternion localRotation;

       
        void Awake()
        {
            Debug.Log("FPS Player Awake");
            cam = Instantiate(FPSCameraPrefab, transform); //Parenting cam to the FPSPlayer
        }

        void Start()
        {
            if (!IsOwner) { cam.enabled = false; }
            Debug.Log("FPS Player Start");
            moveSpeed *= 0.1f;




            //Init Transform Variables
            translation = transform.position;
            rotation = transform.rotation;
            localTranslation = transform.localPosition;
            localRotation = transform.localRotation;
        }

        void OnEnable()
        {

        }
        void OnDisable()
        {

        }


        void Update()
        {
            KeyboardInputs();
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