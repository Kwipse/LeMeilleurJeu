using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : ScriptableObject
{
	public static GameObject[] ClientPlayer = new GameObject[10];	
	
	public static void AddPlayerObject(ulong clientId, GameObject go)
	{
		Debug.Log("Player " + clientId + "has join");
		ClientPlayer[clientId] = go;
	}
	
	public static void RemovePlayerObject(ulong clientId)
	{
		ClientPlayer[clientId] = null;
	}
	
	public static GameObject GetPlayerObject(ulong clientId)
	{
		
		GameObject go = ClientPlayer[clientId];
		return go;
	}
	
	
}
