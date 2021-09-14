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



        void Start()
        {
            //Init FPS Camera
            fcam = Instantiate(FPSCameraPrefab,transform); //Parenting fcam to the FPSPlayer
                                        
        }




        void Update()
        {

        }

    }
}