using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class ModeManager : SyncedBehaviour, IWaitForGameSync
{
	//Declarations
	ulong localId;
	bool PlayMode;
	
	//Init Strings names for PrefabManager
	string FPS = "FPSPlayer";
	string RTS = "RTSPlayer";

    public override void StartAfterGameSync()
    {
        if (!IsOwner) {
            enabled = false; 
            return; }

        //Spawn FPS Player
        PlayMode = true;
        SpawnManager.SpawnPlayer(FPS,Vector3.zero);
    }


	void Update()
	{
        KeyBoardInput();   
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
            //Debug.Log($"Client {NetworkManager.Singleton.LocalClientId} has switched mode");
    }
    
}
