using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LeMeilleurJeu
{
    public class RTSCamera : MonoBehaviour
    {
        public static RTSCamera MyRTSCamera;

        public float MouseSensitivity
        {
            get { return msensitivity; }
            set { msensitivity = value; }
        }
        public float KeybordSensitivity
        {
            get { return ksensitivity; }
            set { ksensitivity = value; }
        }
        public float ZoomSensitivity
        {
            get { return zsensitivity; }
            set { zsensitivity = value; }
        }

        [Range(0.1f, 9f)] [SerializeField] float msensitivity = 2f;
        [Range(0.1f, 9f)] [SerializeField] float ksensitivity = 2f;
        [Range(0.1f, 9f)] [SerializeField] float zsensitivity = 2f;


        Vector3 translation ;
        Vector3 localTranslation;
        Quaternion rotation ;
        Quaternion localRotation;
    



        void Start()
        {
            MyRTSCamera = this;

            translation = transform.position;
            rotation = transform.rotation;
            localTranslation = transform.localPosition;
            localRotation = transform.localRotation;
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
                translation.x += transform.right.x *  ksensitivity;
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
                rotation *= Quaternion.Euler(Vector3.up * ksensitivity *3);
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotation *= Quaternion.Euler(Vector3.up * -ksensitivity*3);
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
            transform.position = translation;
            //transform.localPosition = localTranslation;

            //Apply Rotations
            transform.rotation = rotation;
            //transform.localRotation = localRotation;

        }

        
    }
}

