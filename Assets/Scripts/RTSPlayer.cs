using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace LeMeilleurJeu
{
    public class RTSPlayer : NetworkBehaviour
    {
        //Declarations
        public Camera RTSCameraPrefab;
        Camera rcam;

        public GameObject prefab;
        public Animator anim;



        void Start()
        {
            //Init RTS Camera
            rcam = Instantiate(RTSCameraPrefab,transform); //Parenting rcam to the RTSPlayer
            rcam.transform.position = new Vector3(0, 100, -20);

            anim = GetComponent<Animator>();

        }                     


                                                                                                                                                                     
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                // on se met en mode construction
                anim.SetBool("ConstructionParam", true );

            }


        }
    }
}

