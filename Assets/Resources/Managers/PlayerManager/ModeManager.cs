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
	
    void Awake()
    {
    }
	
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) {
            enabled=false; 
            return; }

    }

    void Start()
    {
        if (IsHost) {
            StartPlayer();
            return; }

        //If not host
        if (!ObjectManager.isSynced) {
            //Debug.Log($"ModeManager : ObjectManager is not synced yet");
            ObjectManager.ObjectManagerSynchronizedEvent += OnObjectManagerSynced; }
    }

    void OnObjectManagerSynced()
    {
        //Debug.Log($"ModeManager : ObjectManager is synced");
        ObjectManager.ObjectManagerSynchronizedEvent -= OnObjectManagerSynced;
        StartPlayer();
    }

    void StartPlayer()
    {
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
