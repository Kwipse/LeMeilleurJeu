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
        ulong localId;

        //Declarations Cam
        public Camera RTSCameraPrefab;
        Camera cam;


        //Trucs ??
        Animator anim;





        void Awake()
        {
            cam = Instantiate(RTSCameraPrefab, transform);
            cam.transform.position = new Vector3(0, 30, -30);
            anim = GetComponent<Animator>();

        }

        void Start()
        {
            //Init RTS Camera
            if (!IsOwner) { cam.enabled = false; } //Disable Camera for non-onwners 
 

            //Init Client ID
            localId = NetworkManager.Singleton.LocalClientId;


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
            MouseInputs();
        }


        void KeyboardInputs()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // on se met en mode construction
                anim.SetBool("ConstructionParam", true);

            }
        }

        void MouseInputs()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                if (plane.Raycast(ray, out float entry))
                {
                    SpawnAtLocation("HumanBox", ray.GetPoint(entry), localId);
                }
            }
        }





        //SPAWNER (obj,loc,id)
        void SpawnAtLocation(string PrefabName, Vector3 SpawnLocation, ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer) 
            {

                //Get the prefab by it's name
                GameObject ObjectToSpawn = (GameObject) Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, Quaternion.identity);
                
                //Spawn object
                ObjectToSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

            }
            else { RequestSpawnAtLocationServerRPC(PrefabName, SpawnLocation, clientId); }
        
        }

        
        
        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnAtLocationServerRPC(string PrefabName, Vector3 SpawnLocation, ulong clientId)
        {
            
            SpawnAtLocation(PrefabName, SpawnLocation, clientId);
        }
        



    }
}

