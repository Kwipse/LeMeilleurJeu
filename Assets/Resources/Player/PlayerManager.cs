using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;


public class PlayerManager : NetworkBehaviour
{
	

	//PUBLIC METHOD TO SPAWN
	public void Spawn(string PlayerPrefabName, Vector3 SpawnLocation, ulong clientId)
	{
		if (NetworkManager.Singleton.IsServer)		
			{SpawnPlayer(PlayerPrefabName, SpawnLocation, clientId);}
		else 			
			{RequestSpawnServerRPC(PlayerPrefabName, SpawnLocation, clientId);}
	}
	
	
	//RPC Spawn Request
	[ServerRpc]
	void RequestSpawnServerRPC(string PlayerPrefabName, Vector3 SpawnLocation, ulong clientId)
	{
		SpawnPlayer(PlayerPrefabName,SpawnLocation,clientId);
	}	
	
	//Spawn 
	void SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation, ulong clientId)
	{
		//Destroy current player if it exist
		GameObject go = PlayerList.GetPlayerObject(clientId);
		if (go != null)
		{
			Destroy(go);
			PlayerList.RemovePlayerObject(clientId);
			Debug.Log("Player " + clientId + " has been destroyed");
		}
		
		//Instantiate and spawn
		go = Instantiate(PrefabManager.GetPrefab(PlayerPrefabName), SpawnLocation, Quaternion.identity);
		go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
		
		//Register in Player List
		PlayerList.AddPlayerObject(clientId,go);
	}
	
	
	
	
	
	//PUBLIC METHOD TO DESTROY
	public void DestroyPlayer(ulong clientId)
	{
		if (NetworkManager.Singleton.IsServer)
			{DestroyObject(clientId);}
		else
			{RequestDestroyServerRPC(clientId);}
	}
	
	//RPC Request to destroy
	[ServerRpc]
	void RequestDestroyServerRPC(ulong clientId)
	{
		DestroyObject(clientId);
	}
	
	//Destroy
	void DestroyObject(ulong clientId)
	{
		Destroy(PlayerList.GetPlayerObject(clientId));
	}
	
}
