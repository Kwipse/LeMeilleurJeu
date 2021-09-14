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



        void Start()
        {
            //Init RTS Camera
            rcam = Instantiate(RTSCameraPrefab,transform); //Parenting rcam to the RTSPlayer
            rcam.transform.position = new Vector3(0, 100, -20);
        }                     


                                                                                                                                                                     
        void Update()
        {

        }



    }
}

