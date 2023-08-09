using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SpawnManager : NetworkBehaviour
{

	
	//SPAWN
	public void Spawn(string PrefabName, Vector3 SpawnLocation, Quaternion SpawnRotation)
		{SpawnServerRpc(PrefabName, SpawnLocation, SpawnRotation);}
		
	[ServerRpc]
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
	public void Destroy(NetworkObject no)
		{DestroyServerRPC(no);}
	
	[ServerRpc]
	void DestroyServerRPC(NetworkObjectReference no)	
		{Destroy(no);}
	
	
	
	
	//SPAWN PLAYER
	public void SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation)
		{SpawnPlayerServerRPC(PlayerPrefabName, SpawnLocation);}
		
	[ServerRpc]
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
	[ServerRpc]
	void DestroyPlayerServerRPC(ServerRpcParams serverRpcParams = default)
	{ 
		var clientId = serverRpcParams.Receive.SenderClientId;
		DestroyPlayer(clientId);
	}
	
	public void DestroyPlayer(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer){DestroyPlayerServerRPC();}
			
		else
			{Destroy(PlayerList.PlayerListinstance.GetPlayerObject(clientId));}
	}
	
	
	
	/*
	public void SomeServerRpc(ServerRpcParams serverRpcParams = default)
	{
    var clientId = serverRpcParams.Receive.SenderClientId;
	}
	*/
}
