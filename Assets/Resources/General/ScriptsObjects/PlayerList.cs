using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;



public class PlayerList : MonoBehaviour// NetworkBehaviour
{
	
	public  GameObject[] ClientPlayer = new GameObject[10];	
	
	//WakeUp Mr. PlayerList
	public static PlayerList PlayerListinstance { get; private set; }	
	private void Awake() 
	{ 

		// If there is an PlayerListinstance, and it's not me, delete myself.
		
		if (PlayerListinstance != null && PlayerListinstance != this) 
		{ 
			Destroy(this); 
		} 
		else 
		{ 
			PlayerListinstance = this; 
		} 
	}
	
	/*public override void NetworkStart()
	{
		if (!NetworkManager.Singleton.IsServer) {Destroy(this); }
		
	}*/
	

	
	
	public  void AddPlayerObject(ulong clientId, GameObject go)
	{
		Debug.Log("Player " + clientId + " has join");
		ClientPlayer[clientId] = go;
	}
	
	public  void RemovePlayerObject(ulong clientId)
	{
		Debug.Log("Player " + clientId + " has been destroyed");
		ClientPlayer[clientId] = null;
	}
	
	public  GameObject GetPlayerObject(ulong clientId)
	{
		//Debug.Log("Player " + clientId + " gameObject has been found");
		GameObject go = ClientPlayer[clientId];
		return go;
	}
	
	
}
