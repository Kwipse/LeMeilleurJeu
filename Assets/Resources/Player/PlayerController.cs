using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

    
public class PlayerController : NetworkBehaviour
{
	//Declarations
	SpawnManager SM;
	
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
		
			//Init the player manager
			SM = GetComponent<SpawnManager>();
			
			//Disable the main camera, just in case
			Camera.main.enabled = false;
			
			//Get ClientID
			localId = NetworkManager.Singleton.LocalClientId;
						
			//Spawn FPS Player
			PlayMode = true;
			SM.SpawnPlayer(FPS,Vector3.zero, localId);
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
		//lol
			PlayMode = !PlayMode;
			if (PlayMode) {SM.SpawnPlayer(FPS,Vector3.zero, localId);}				
			else 	  	  {SM.SpawnPlayer(RTS,Vector3.zero, localId);}
	}

}