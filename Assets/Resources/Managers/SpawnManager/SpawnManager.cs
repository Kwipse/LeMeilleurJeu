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
		


    //SPAWN UNIT
    public static void SpawnUnit(
            GameObject unitPrefab, Vector3 spawnPosition,
            Quaternion spawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3)) 
    { 
        SM.SpawnUnitByNameServerRpc(unitPrefab.name, spawnPosition, spawnRotation, rallyPosition);
    }

    public static void SpawnUnitByName(
            string unitPrefabName, Vector3 spawnPosition,
            Quaternion spawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3))
    { 
        SM.SpawnUnitByNameServerRpc(unitPrefabName, spawnPosition, spawnRotation, rallyPosition);
    }

	[ServerRpc(RequireOwnership = false)]
	void SpawnUnitByNameServerRpc(
            string PrefabName, Vector3 SpawnLocation,
            Quaternion SpawnRotation = default(Quaternion), Vector3 rallyPosition = default(Vector3),
            ServerRpcParams serverRpcParams = default)
	{
		var clientId = serverRpcParams.Receive.SenderClientId;

		GameObject go = Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, SpawnRotation);
		go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

        //if (rallyPosition != default(Vector3))
            SM.MoveUnitToRallyPointClientRPC(go, rallyPosition);
	}

    [ClientRpc]
    void MoveUnitToRallyPointClientRPC(
            NetworkObjectReference nor, Vector3 rallyPosition,
            ClientRpcParams clientRpcParams = default)
    {
        NetworkObject no = nor;
        GameObject go = nor;

        Debug.Log($"SpawnManager : Received order to move {go.name} to rally point {rallyPosition}");

        if (no.IsOwner)
            go.GetComponent<UnitSystem>().MoveUnitToPos(rallyPosition, false);
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
        
        if (!no) {
            Debug.Log($"SpawnManager : The object you are trying to destroy does not exist"); 
            return;
        }

        no.Despawn();
        Debug.Log($"SpawnManager : I despawned {no.name} for client {clientId}");
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
		//Debug.Log("PlayerId :" + clientId + " - go :" + go);
		if (go != null) 
            DestroyPlayer(go);
		
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
        GameObject go = nor;
        NetworkObject no = nor;
        var playerId = PlayerList.PlayerListinstance.GetPlayerId(go);

        no.Despawn();
        PlayerList.PlayerListinstance.RemovePlayerObject((ulong) playerId);

        //Debug.Log("Player " + playerId + " has been destroyed");

        //GameObject playerObjectToDestroy = PlayerList.PlayerListSM.GetPlayerObject(clientId);
        //Destroy(playerObjectToDestroy);
        //NetworkObject NetworkObjectToDespawn = playerObjectToDestroy.GetComponent<NetworkObject>();
	}
	
}
