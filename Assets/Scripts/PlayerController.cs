using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace LeMeilleurJeu
{
    public class PlayerController : NetworkBehaviour
    {
        bool FPSMode ;
        bool RTSMode ;



        //Prefabs
        public GameObject FPSPlayerPrefab;
        public GameObject RTSPlayerPrefab;

        GameObject FPSPlayerInstance;
        GameObject RTSPlayerInstance;
        




        // Start is called before the first frame update
        void Start()
        {

            FPSMode = true;
            RTSMode = false;

           

            if (NetworkManager.Singleton.IsClient)
            {
                if (IsOwner)
                {
                    Camera.main.enabled = false; //disable current camera
                    if (FPSMode)
                    {
                        FPSPlayerInstance = Instantiate(FPSPlayerPrefab, Vector3.zero, Quaternion.identity);
                    }
                    if (RTSMode)
                    {
                        RTSPlayerInstance = Instantiate(RTSPlayerPrefab, Vector3.zero, Quaternion.identity);
                       
                    }
                }
            }

        }



        // Update is called once per frame
        void Update()
        {
         
        }



        public void SwitchMode()
        {
         
            if (NetworkManager.Singleton.IsClient)
            {
                if (IsOwner)
                {
                    FPSMode = !FPSMode;
                    RTSMode = !RTSMode;

                    if (FPSMode)
                    {
                        Destroy(RTSPlayerInstance);
                        FPSPlayerInstance = Instantiate(FPSPlayerPrefab, Vector3.zero, Quaternion.identity);
                    }


                    if (RTSMode)
                    {
                        Destroy(FPSPlayerInstance);
                        RTSPlayerInstance = Instantiate(RTSPlayerPrefab, Vector3.zero, Quaternion.identity);
                    }

                }
            }
        }

    }
}
