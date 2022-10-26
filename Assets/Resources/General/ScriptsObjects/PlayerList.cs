using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Singleton : MonoBehaviour
{
	
	public  GameObject[] ClientPlayer = new GameObject[10];	
	
	//WakeUp Mr. Singleton
	public static Singleton PlayerList { get; private set; }	
	private void Awake() 
	{ 
		// If there is an PlayerList, and it's not me, delete myself.
		
		if (PlayerList != null && PlayerList != this) 
		{ 
			Destroy(this); 
		} 
		else 
		{ 
			PlayerList = this; 
		} 
	}
	

	
	
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
		Debug.Log("Player " + clientId + " gameObject has been found");
		GameObject go = ClientPlayer[clientId];
		return go;
	}
	
	
}
