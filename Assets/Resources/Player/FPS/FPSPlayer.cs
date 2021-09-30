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
        Camera cam;

        Vector3 translation;

        Rigidbody RBody;
        Animator anim;

        float jumpForce;
        float moveSpeed = 1f;



       public override void NetworkStart()
       {

            if (IsOwner)
            {
				cam = Instantiate(FPSCameraPrefab, transform);
                cam.transform.position = new Vector3(0, 5, 0);
                anim = GetComponent<Animator>();
                anim.SetBool("IsMoving", false);



                moveSpeed *= 0.05f;


                //Init Transform Variables
                translation = Vector3.zero;
            }
            
       }


        void Update()
        {
            if (IsOwner)
            {
                KeyboardInputs();
                MoveFPSPlayer();
            }
        }



        void KeyboardInputs()
        {
            // ZQSD or Arrow keys
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) { translation += transform.forward * moveSpeed; }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { translation -= transform.forward * moveSpeed; }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { translation += transform.right * moveSpeed; }
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) { translation -= transform.right * moveSpeed; }

        }


        void MoveFPSPlayer()
        {
            //Move
            transform.localPosition += translation;

            if (translation == Vector3.zero) 
            { 
                anim.SetBool("IsMoving", false); 
            }
            else                                 
            { 
                anim.SetBool("IsMoving", true);
                translation = Vector3.zero;
            }
            
        }


    }
}