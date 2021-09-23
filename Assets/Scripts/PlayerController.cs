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

        //Import Prefabs
        public GameObject FPSPlayerPrefab;
        public GameObject RTSPlayerPrefab;

        GameObject[] ClientPlayer = new GameObject[10];



        public override void NetworkStart()
        {
            if (IsOwner)
            {
                Debug.Log("PLAYER CONTROLLER : NETWORKSTART ");

                // Get Local ID
                localId = NetworkManager.Singleton.LocalClientId;

                // Init Mode
                PlayMode = true;

                //Disable current camera
                Camera.main.enabled = false;

                //Load prefabs on the server
                if (NetworkManager.Singleton.IsServer) { PrefabManager.LoadAllPrefabs("Prefabs"); }



                SpawnPlayer(localId, PlayMode);

            }
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
            Debug.Log("Spawning with PlayMode = " + mode + " for Client " + targetId);

            //Instantiate
            if (mode)
            {
                ClientPlayer[targetId] = Instantiate(FPSPlayerPrefab, Vector3.up, Quaternion.identity);
                Debug.Log("ClientPlayer[" + targetId + "] = FPS");
            }
            else
            {
                ClientPlayer[targetId] = Instantiate(RTSPlayerPrefab, Vector3.up, Quaternion.identity);
                Debug.Log("ClientPlayer[" + targetId + "] = RTS");
            }
            
            //Spawn
            ClientPlayer[targetId].GetComponent<NetworkObject>().SpawnWithOwnership(targetId); 
        }

        [ServerRpc]
        public void RequestSpawnPlayerServerRPC(ulong clientId, bool mode)
        {
            Debug.Log("SpawnRPC from Client " + clientId);
            ServerSpawnPlayer(clientId, mode);
        }




        //DESPAWN

        void DespawnPlayer(ulong playerId)
        {
            if (NetworkManager.Singleton.IsServer) { ServerDespawnPlayer(playerId); }
            else                                   { RequestDespawnPlayerServerRPC(playerId); }
         
        }
        void ServerDespawnPlayer(ulong playerId)
        {
            ClientPlayer[playerId].GetComponent<NetworkObject>().Despawn(); Debug.Log("Despawned Player " + playerId);
        }

        [ServerRpc]
        public void RequestDespawnPlayerServerRPC(ulong clientId)
        {
            Debug.Log("DespawnRPC from Client " + clientId);
            ServerDespawnPlayer(clientId);
        }




        //DESTROY

        void DestroyPlayer(ulong playerId)
        {
            if (NetworkManager.Singleton.IsServer) { ServerDestroyPlayer(playerId); }
            else                                   { RequestDestroyPlayerServerRPC(playerId); }
        }

        void ServerDestroyPlayer(ulong playerId)
        {
            Destroy(ClientPlayer[playerId]); Debug.Log("Destroyed Player " + playerId);
        }

        [ServerRpc]
        public void RequestDestroyPlayerServerRPC(ulong clientId)
        {
            Debug.Log("DestroyRPC from Client " + clientId);
            ServerDestroyPlayer(clientId);
        }


    }

}
