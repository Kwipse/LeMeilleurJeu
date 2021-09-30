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
        bool PlayMode;
        ulong localId;

        GameObject[] ClientPlayer = new GameObject[10];


		
        public override void NetworkStart()
        {

            if (IsOwner)
            {
				if (NetworkManager.Singleton.IsServer) { PrefabManager.LoadAllPrefabs(); } //Init PrefabManager
				
                localId = NetworkManager.Singleton.LocalClientId;
                PlayMode = true; //True : FPS  -   False : RTS

                //Disable the main camera, just in case
                Camera.main.enabled = false;

                SpawnPlayer(localId, PlayMode);
            }
        }


        void Update()
        {
            KeyBoardInput();            
        }


        void KeyBoardInput()
        {
            if(Input.GetKeyDown(KeyCode.Tab)) { SwitchMode(); }
        }


        //PlayMode Switcher
        public void SwitchMode()
        {
            if (IsOwner)
            {

                PlayMode = !PlayMode;

                DestroyPlayer(localId);
                SpawnPlayer(localId, PlayMode);
            }
        }



        //SPAWN

        void SpawnPlayer(ulong targetId, bool mode)
        {
            if (NetworkManager.Singleton.IsServer) { ServerSpawnPlayer(targetId, mode); }
            else                                   { RequestSpawnPlayerServerRPC(targetId, mode); }           
        }

        void ServerSpawnPlayer(ulong targetId, bool mode)
        {
Debug.Log("Ohélskdfmqskdj");
            //Instantiate
            if (mode) { ClientPlayer[targetId] = Instantiate(PrefabManager.GetPrefab("FPSPlayer"), Vector3.zero, Quaternion.identity); }
            else      { ClientPlayer[targetId] = Instantiate(PrefabManager.GetPrefab("RTSPlayer"), Vector3.zero, Quaternion.identity); }
            
            //Spawn
            ClientPlayer[targetId].GetComponent<NetworkObject>().SpawnWithOwnership(targetId); 
        }

        [ServerRpc]
        public void RequestSpawnPlayerServerRPC(ulong clientId, bool mode)
        {
            Debug.Log("SpawnRPC from Client " + clientId);
            ServerSpawnPlayer(clientId, mode);
        }




        //DESTROY

        void DestroyPlayer(ulong playerId)
        {
            if (NetworkManager.Singleton.IsServer) { ServerDestroyPlayer(playerId); }
            else                                   { RequestDestroyPlayerServerRPC(playerId); }
        }

        void ServerDestroyPlayer(ulong playerId)
        {
            Destroy(ClientPlayer[playerId]);
        }

        [ServerRpc]
        public void RequestDestroyPlayerServerRPC(ulong clientId)
        {
            ServerDestroyPlayer(clientId);
        }


    }

}
