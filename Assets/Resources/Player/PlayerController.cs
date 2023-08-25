using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

    
public class PlayerController : NetworkBehaviour
{
	//Declarations
	
	ulong localId;
	bool PlayMode;
	

	//Init Strings names for PrefabManager
	string FPS = "FPSPlayer";
	string RTS = "RTSPlayer";
	string toSpawn ;
	
	
	public override void OnNetworkSpawn()
	{
		if (!IsOwner) {enabled=false;}
	}

    void Start()
    {
        //Init the player manager

        //Disable the main camera, just in case
        Camera.main.enabled = false;

        //Spawn FPS Player
        PlayMode = true;

        //Tests
        SpawnManager.spawner.SpawnPlayer(FPS,Vector3.zero);
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
			PlayMode = !PlayMode;
			if (PlayMode) {SpawnManager.spawner.SpawnPlayer(FPS,Vector3.zero);}				
			else 	  	  {SpawnManager.spawner.SpawnPlayer(RTS,Vector3.zero);}
    }
    
}
