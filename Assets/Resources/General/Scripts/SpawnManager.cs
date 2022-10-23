using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class SpawnManager : NetworkBehaviour
{

	
	//PUBLIC METHOD TO CALL
	public void Spawn(string PrefabName, Vector3 SpawnLocation, ulong clientId)
	{
		if (NetworkManager.Singleton.IsServer) 
			{SpawnPrefab(PrefabName, SpawnLocation, clientId);}
		else
			{RequestSpawnServerRPC(PrefabName, SpawnLocation, clientId);}
	}
	
	
	//RPC Spawn Request
	[ServerRpc]
	void RequestSpawnServerRPC(string PrefabName, Vector3 SpawnLocation, ulong clientId)
	{
		SpawnPrefab(PrefabName, SpawnLocation, clientId);
	}
	
	//Spawn
	void SpawnPrefab(string PrefabName, Vector3 SpawnLocation, ulong clientId)
	{
		if (NetworkManager.Singleton.IsServer) 
		{
			GameObject go = Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, Quaternion.identity);
			go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
		}

	}
	
}
