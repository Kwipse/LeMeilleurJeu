using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

using UnityEngine;

    
public class PlayerController : NetworkBehaviour
{
	//Declarations
	PlayerManager PM;
	ulong localId;
	bool PlayMode;
	

	//Init Strings names for PrefabManager
	string FPS = "FPSPlayer";
	string RTS = "RTSPlayer";
	string toSpawn ;
	
	
	public override void OnNetworkSpawn()
	{
		if (!IsOwner) {enabled=false;}
		else
		{
			//Init the prefab manager 
			if (NetworkManager.Singleton.IsServer) { PrefabManager.LoadAllPrefabs(); }

			//Init the player manager
			PM = GetComponent<PlayerManager>();
			
			//Disable the main camera, just in case
			Camera.main.enabled = false;
			
			//Get ClientID
			localId = NetworkManager.Singleton.LocalClientId;
						
			//Spawn Player
			PlayMode = true;
			SpawnPlayer();
		}
	}


	void Update()
	{
		if (IsOwner) 
		{ 
			KeyBoardInput();   
		}         
	}


	void KeyBoardInput()
	{
		if(Input.GetKeyDown(KeyCode.Tab)) { SwitchMode(); }
	}



	//PlayMode Switcher
	public void SwitchMode()
	{
		if (IsOwner)
		{
			PlayMode = !PlayMode;
			SpawnPlayer();
		}
	}

	
	void SpawnPlayer()
	{
		if (IsOwner)
		{
			if (PlayMode) {toSpawn = FPS;}				
			else 	  	  {toSpawn = RTS;}
			PM.Spawn(toSpawn,Vector3.zero, localId);
		}
	}
}