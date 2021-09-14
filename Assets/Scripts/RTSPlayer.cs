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



        void Start()
        {
            //Init RTS Camera
            rcam = Instantiate(RTSCameraPrefab,transform); //Parenting rcam to the RTSPlayer
            rcam.transform.position = new Vector3(0, 100, -20);
        }                     


                                                                                                                                                                     
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
            
                if(Physics.Raycast(ray, out hit, 100.0f))
                {
                    //creer le prefab au point de collision.
                    GameObject go = Instantiate(prefab, hit.point, Quaternion.identity );
                    Debug.Log("trtsplayer_ownership: "+go.GetComponent<NetworkObject>().IsOwner );
                    //si c'est lhote fait un spawn
                    if (NetworkManager.Singleton.IsServer)
                    {
                        go.GetComponent<NetworkObject>().Spawn();  
                    }
                    //si cest le client fait une demande à l'hote
                    else
                    {
                        
                    }
                }
            }


        }
    }
}

