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

    public override void StartAfterGameSync()
    {
        if (!IsOwner) {
            enabled = false; 
            return; }

        //Spawn FPS Player
        PlayMode = FPS ;
        SpawnFPS();
        //SpawnManager.SpawnPlayer(FPS,Vector3.zero);
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
        string currentPlayMode = PlayMode;
        if(currentPlayMode == RTS) { PlayMode=FPS; }
        if(currentPlayMode == FPS) { PlayMode=RTS; }

        if (PlayMode == FPS) {SpawnFPS();}
        if (PlayMode == RTS) {SpawnRTS();}
    }

    public void BecomeGhost()
    {
        PlayMode = Ghost;
        SpawnGhost();
        Invoke("Revive",GhostDuration);
    }

    public void Revive()
    {
        PlayMode = FPS;
        SpawnFPS();
    }


    void SpawnFPS()
    {
        List<GameObject> spawners = TeamManager.GetPlayerSpawners(OwnerClientId);
        if (spawners.Count != 0) { SpawnManager.SpawnPlayer(FPS,spawners[0].transform.position); }
        if (spawners.Count == 0) { SpawnManager.SpawnPlayer(FPS,Vector3.zero); }
    }

    void SpawnRTS()
    {
        SpawnManager.SpawnPlayer(RTS,Vector3.zero);
    }

    void SpawnGhost()
    {
        SpawnManager.SpawnPlayer(Ghost, PlayerManager.GetPlayerObject(OwnerClientId).transform.position);
    }
}
