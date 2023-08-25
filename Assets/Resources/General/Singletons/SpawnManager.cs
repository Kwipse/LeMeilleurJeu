using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SpawnManager : NetworkBehaviour
{
    //init global access as SpawnManager.spawner.function()
    public static SpawnManager spawner;
    
    void Awake()
    { 
        // If there is an spawner, and it's not me, delete myself.
        if (spawner != null && spawner != this) 
        { Destroy(this); } 
        else 
        { spawner = this; } 
    }
    
	public override void OnNetworkSpawn()
	{ if (!IsOwner) { enabled=false; } }
    


	//SPAWN
	public void Spawn(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation)
		{SpawnServerRpc(PrefabName, SpawnLocation, SpawnRotation);}
		
	[ServerRpc(RequireOwnership = false)]
	void SpawnServerRpc(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation, ServerRpcParams serverRpcParams = default)
	{
		var clientId = serverRpcParams.Receive.SenderClientId;
		GameObject go = Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, SpawnRotation);
		go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
		Debug.Log(go+" has been spawned for client "+clientId);	
	}
		
	

	
	//DESTROY CUBEDELAMORT
	[ServerRpc(RequireOwnership = false)]
	public void DestroyCubeServerRpc(int dureeOuiNon =0 )
	{
		//GetComponent<Netw>
		if (dureeOuiNon == 0)
		{
			Destroy(gameObject);
		}else
			Destroy(gameObject,10);

    }
	
	
	
	/*/DESPAWN
	public void Despawn(NetworkObject no)
		{DespawnServerRPC(no);}
		
	[ServerRpc]
	void DespawnServerRPC(NetworkObjectReference no)	
		{no.Despawn();}
	//*/

	
	
    //DESTROY
	public void DestroyObject(GameObject go, int secondsBeforeDestroy = 0)
        {DestroyServerRPC(go, secondsBeforeDestroy);}

	
	[ServerRpc(RequireOwnership = false)]
	void DestroyServerRPC(NetworkObjectReference nor, int secondsBeforeDestroy)	
    {
        NetworkObject no = nor;
        no.Despawn();
    }


	
	
	//SPAWN PLAYER
	public void SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation)
		{SpawnPlayerServerRPC(PlayerPrefabName, SpawnLocation);}
		
	[ServerRpc(RequireOwnership = false)]
	void SpawnPlayerServerRPC(string PlayerPrefabName, Vector3 SpawnLocation, ServerRpcParams serverRpcParams = default)
	{
		var clientId = serverRpcParams.Receive.SenderClientId;
		
		//Destroy current player if it exist
		GameObject go = PlayerList.PlayerListinstance.GetPlayerObject(clientId);

		Debug.Log("client id :"+clientId+" - go :"+go);
		if (go != null)
		{
			Destroy(go);
			PlayerList.PlayerListinstance.RemovePlayerObject(clientId);
			Debug.Log("Player " + clientId + " has been destroyed");
		}
		
		//Instantiate and spawn
		go = Instantiate(PrefabManager.GetPrefab(PlayerPrefabName), SpawnLocation, Quaternion.identity);
		go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
		
		//Register in Player List
		PlayerList.PlayerListinstance.AddPlayerObject(clientId,go);
	}
	
	
	//DESTROY PLAYER
    public void DestroyPlayer()
        {DestroyPlayerServerRPC();}
    
	[ServerRpc(RequireOwnership = false)]
	void DestroyPlayerServerRPC(ServerRpcParams serverRpcParams = default)
	{ 
		var clientId = serverRpcParams.Receive.SenderClientId;
        GameObject playerObjectToDestroy = PlayerList.PlayerListinstance.GetPlayerObject(clientId);
		Destroy(playerObjectToDestroy);
	}
	
}
