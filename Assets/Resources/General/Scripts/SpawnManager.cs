using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SpawnManager : MonoBehaviour
{

	
	//PUBLIC METHOD TO CALL
	public void Spawn(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
		if (NetworkManager.Singleton.IsServer) 
			{SpawnPrefab(PrefabName, SpawnLocation, clientId, SpawnRotation);}
		else
			{RequestSpawnServerRPC(PrefabName, SpawnLocation, clientId, SpawnRotation);}
	}
	
	
	//RPC Spawn Request
	[ServerRpc]
	void RequestSpawnServerRPC(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
			{SpawnPrefab(PrefabName, SpawnLocation, clientId, SpawnRotation);}
	}
	
	//Spawn
	void SpawnPrefab(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
		if (NetworkManager.Singleton.IsServer) 
		{
			GameObject go = Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, SpawnRotation);
			go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
		}

	}
	
}
