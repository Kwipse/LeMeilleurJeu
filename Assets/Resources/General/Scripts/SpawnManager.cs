using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SpawnManager : NetworkBehaviour
{

	
	//Spawn
	public void Spawn(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
		if (NetworkManager.Singleton.IsServer) 
			{ServerSpawn(PrefabName, SpawnLocation, clientId, SpawnRotation);}
		else
			{RequestSpawnServerRpc(PrefabName, SpawnLocation, clientId, SpawnRotation);}
	}
	
	
	//Spawn RPC
	[ServerRpc]
	void RequestSpawnServerRpc(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
			{ServerSpawn(PrefabName, SpawnLocation, clientId, SpawnRotation);}
	}
	
	//ServerSpawn
	void ServerSpawn(string PrefabName, Vector3 SpawnLocation, ulong clientId, Quaternion SpawnRotation)
	{
		if (NetworkManager.Singleton.IsServer) 
		{
			GameObject go = Instantiate(PrefabManager.GetPrefab(PrefabName), SpawnLocation, SpawnRotation);
			go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
			Debug.Log(go+" has been spawned for client "+clientId);
		}
	}
	
	
	
	
	
	
	//Destroy
	public void Destroy(NetworkObject no)
	{
		if (NetworkManager.Singleton.IsServer) 
			{/*ServerDestroy(GameObject go);*/
				no.Despawn();
			}
		else
			{RequestDestroyServerRPC(no);}
	}
	
	//Destroy RPC
	[ServerRpc]
	void RequestDestroyServerRPC(NetworkObjectReference no)	{Destroy(no);}
	
	
	/*/ServerDestroy
	void ServerDestroy(GameObject go)
	{
		if (NetworkManager.Singleton.IsServer) 
		{
			go.Despawn();
		}
	}
	/*/
}
