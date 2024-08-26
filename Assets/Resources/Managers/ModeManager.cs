using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class ModeManager : SyncedBehaviour, IWaitForGameSync
{
	//Declarations
	ulong localId;
	string PlayMode;
	
	//Init Strings names for PrefabManager
	string FPS = "FPSPlayer";
	string RTS = "RTSPlayer";
    string Ghost = "GhostPlayer";

    // ghost
    float GhostDuration = 3.0f;
    float GhostTimer;

    public override void StartAfterGameSync()
    {
        if (!IsOwner) {
            enabled = false; 
            return; }

        //Spawn FPS Player
        PlayMode = FPS ;
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
        if(PlayMode == RTS)PlayMode=FPS;
        if(PlayMode == FPS)PlayMode=RTS;

        if (PlayMode == FPS) {SpawnManager.SpawnPlayer(FPS,Vector3.zero);}
        if (PlayMode == FPS) {SpawnManager.SpawnPlayer(RTS,Vector3.zero);}
        //Debug.Log($"Client {NetworkManager.Singleton.LocalClientId} has switched mode");
    }

    public void BecomeGhost()
    {
        PlayMode = Ghost;
        GhostTimer = Time.time + GhostDuration ;
        SpawnManager.SpawnPlayer(Ghost,Vector3.zero);
        Invoke("Revive",GhostDuration);
    }

    public void Revive()
    {
        PlayMode = FPS;
        SpawnManager.SpawnPlayer(FPS,Vector3.zero);
    }
}
