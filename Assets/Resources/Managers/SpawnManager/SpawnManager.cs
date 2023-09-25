using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SpawnManager : NetworkBehaviour
{
    //Enable global access to static 
    //functions as SpawnManager.function()
    static SpawnManager SM;
    void Awake()
    { 
        SM = this; 
        Debug.Log("SpawnManager : J'existe !");
    }
    

    //SPAWN OBJECT
    public static void SpawnObject(GameObject Prefab, Vector3 SpawnLocation, Quaternion SpawnRotation)
    {SM.SpawnObjectByNameServerRpc(Prefab.name, SpawnLocation, SpawnRotation);}

	//SPAWN OBJECT BY NAME
	public static void SpawnObjectByName(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation)
		{SM.SpawnObjectByNameServerRpc(PrefabName, SpawnLocation, SpawnRotation);}
		
	[ServerRpc(RequireOwnership = false)]
	void SpawnObjectByNameServerRpc(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation, ServerRpcParams serverRpcParams = default)
	{
		var clientId = serverRpcParams.Receive.SenderClientId;

		GameObject go = Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, SpawnRotation);
		go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
	}
		

    //DESTROY
	public static void DestroyObject(GameObject go)
        {
            //Debug.Log("I gave the order to destroy " + go.name);
            SM.DestroyServerRPC(go);
        }

	[ServerRpc(RequireOwnership = false)]
	void DestroyServerRPC(NetworkObjectReference nor, ServerRpcParams serverRpcParams = default)	
    {
		var clientId = serverRpcParams.Receive.SenderClientId;

        NetworkObject no = nor;
        //Debug.Log("ServerManager : RPC from client " + clientId + " -> I received the order to despawn " + no.name);
        no.Despawn();
        //Debug.Log("ServerRPC from client " + clientId + " -> I despawned " + no.name);
    }


        

	//SPAWN PLAYER
	public static void SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation)
		{SM.SpawnPlayerServerRPC(PlayerPrefabName, SpawnLocation);}
		
	[ServerRpc(RequireOwnership = false)]
	void SpawnPlayerServerRPC(string PlayerPrefabName, Vector3 SpawnLocation, ServerRpcParams serverRpcParams = default)
	{
		var clientId = serverRpcParams.Receive.SenderClientId;
		
		//Destroy current player if it exist
		GameObject go = PlayerList.PlayerListinstance.GetPlayerObject(clientId);

		//Debug.Log("client id :"+clientId+" - go :"+go);
		if (go != null)
		{
            DestroyPlayer(go);
		}
		
		//Instantiate and spawn
		go = Instantiate(PrefabManager.GetPrefab(PlayerPrefabName), SpawnLocation, Quaternion.identity);
		go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
		
		//Register in Player List
		PlayerList.PlayerListinstance.AddPlayerObject(clientId,go);
	}
	

	
	//DESTROY PLAYER
    public static void DestroyPlayer(GameObject go)
        {SM.DestroyPlayerServerRPC(go);}
    
	[ServerRpc(RequireOwnership = false)]
	void DestroyPlayerServerRPC(NetworkObjectReference nor, ServerRpcParams serverRpcParams = default)
	{ 
		var clientId = serverRpcParams.Receive.SenderClientId;
        NetworkObject no = nor;
        no.Despawn();
        PlayerList.PlayerListinstance.RemovePlayerObject(clientId);

        //Debug.Log("Player " + clientId + " has been destroyed by player " + clientId);

        //GameObject playerObjectToDestroy = PlayerList.PlayerListSM.GetPlayerObject(clientId);
        //Destroy(playerObjectToDestroy);
        //NetworkObject NetworkObjectToDespawn = playerObjectToDestroy.GetComponent<NetworkObject>();
	}
	
}
