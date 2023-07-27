using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerStart : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
		//Do on server start
        if (NetworkManager.Singleton.IsServer) { 
		
			PrefabManager.LoadAllPrefabs(); //Populate PrefabManager
		
		}	
    }
}
