using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SpawnManager : NetworkBehaviour
{

	
	//PUBLIC METHOD TO CALL
	public void Spawn(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
		if (NetworkManager.Singleton.IsServer) 
			{ServerSpawn(PrefabName, SpawnLocation, clientId, SpawnRotation);}
		else
			{RequestSpawnServerRpc(PrefabName, SpawnLocation, clientId, SpawnRotation);}
	}
	
	
	//RPC Spawn Request
	[ServerRpc]
	void RequestSpawnServerRpc(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
			{ServerSpawn(PrefabName, SpawnLocation, clientId, SpawnRotation);}
	}
	
	//Spawn
	void ServerSpawn(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
		if (NetworkManager.Singleton.IsServer) 
		{
			GameObject go = Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, SpawnRotation);
			go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
		}

	}
	
}
