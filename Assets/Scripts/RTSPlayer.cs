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
        Camera cam;

        public GameObject prefab;
        public Animator anim;

        void Awake()
        {
            Debug.Log("RTS Player Awake");
            cam = Instantiate(RTSCameraPrefab, transform); //Parenting cam to the FPSPlayer

        }

        void Start()
        {
            //Init RTS Camera
            if (!IsOwner) { cam.enabled = false; }
            cam.transform.position = new Vector3(0, 30, -30);

            anim = GetComponent<Animator>();

        }

        void OnEnable()
        {

        }
        void OnDisable()
        {

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

