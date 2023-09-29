using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

    
public class ModeManager : NetworkBehaviour
{
	//Declarations
	ulong localId;
	bool PlayMode;
	
	//Init Strings names for PrefabManager
	string FPS = "FPSPlayer";
	string RTS = "RTSPlayer";
	
	
	public override void OnNetworkSpawn()
	{
		if (!IsOwner) {enabled=false;} 
    }

    void Start()
    {
        //Spawn FPS Player
        PlayMode = true;
        SpawnManager.SpawnPlayer(FPS,Vector3.zero);
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
			if (PlayMode) {SpawnManager.SpawnPlayer(FPS,Vector3.zero);}
			else 	  	  {SpawnManager.SpawnPlayer(RTS,Vector3.zero);}
    }
    
}
