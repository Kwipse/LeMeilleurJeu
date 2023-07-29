using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class PlayerSpawner : NetworkBehaviour
{
	

	//SPAWN PLAYER
	
	[ServerRpc]
	void SpawnPlayerServerRPC(string PlayerPrefabName, Vector3 SpawnLocation, ulong clientId)
		{SpawnPlayer(PlayerPrefabName,SpawnLocation,clientId);}		
	
	public void SpawnPlayer(string PlayerPrefabName, Vector3 SpawnLocation, ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer) {SpawnPlayerServerRPC(PlayerPrefabName, SpawnLocation, clientId);}
		else 
		{
			Debug.Log("spawnplayer player list singleton"+PlayerList.PlayerListinstance);
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
	}
	
	

	
	//DESTROY PLAYER
	
	[ServerRpc]
	void DestroyPlayerServerRPC(ulong clientId)
		{DestroyPlayer(clientId);}
	
	public void DestroyPlayer(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer){DestroyPlayerServerRPC(clientId);}
			
		else
			{Destroy(PlayerList.PlayerListinstance.GetPlayerObject(clientId));}
	}
	

}
