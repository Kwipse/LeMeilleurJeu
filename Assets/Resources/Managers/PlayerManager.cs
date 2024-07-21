using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerManager : SyncedBehaviour, ISyncBeforeGame, IWaitForGameSync
{
    public static NetworkList<int> PlayerObjectIDs;
    public static Dictionary<ulong, GameObject> PlayerObjects;
    public static Dictionary<ulong, Camera> PlayerCameras;
    public static Dictionary<ulong, int> PlayerTeams;
	
    //Access these everywhere with PlayerManager.Method();
	static PlayerManager PM;
	private void Awake() 
	{ 
        PM = this;
        PlayerObjectIDs = new NetworkList<int>();
        PlayerObjects = new Dictionary<ulong, GameObject>();
        PlayerCameras = new Dictionary<ulong, Camera>();
        PlayerTeams = new Dictionary<ulong, int>();

        //create local dictionaries
        for (ulong clientID = 0 ; clientID < 10 ; clientID++) {
            PlayerObjects.Add(clientID, null);
            PlayerCameras.Add(clientID, null); 
            PlayerTeams.Add(clientID, -1); }
    }


    public override void InitializeBeforeSync()
    {
        //create and init network list
        if (IsServer) {
            while (PlayerObjectIDs.Count < 10) {
                PlayerObjectIDs.Add(-1); } }

        PlayerObjectIDs.OnListChanged += OnPlayerIDChange; 
    }

    public override void StartAfterGameSync()
    {
        //init dictionaries with the synchronized PlayerObjectIDs infos at client connection
        if (!IsServer)
        {
            for (int playerNum = 0; playerNum < PlayerObjectIDs.Count; playerNum++) {
                if (PlayerObjectIDs[playerNum] >= 0) { //If player has an associated object
                    UpdatePlayer((ulong) playerNum); } }
        }
    }


    void OnPlayerIDChange(NetworkListEvent<int> lastChange)
    {
        //Debug.Log($"{this.GetType()} : PlayerObjectID list modified - Player {lastChange.Index} switched object id from {lastChange.PreviousValue} to {lastChange.Value}");
        if (lastChange.Type.ToString() == "Value") {
            UpdatePlayer((ulong) lastChange.Index); }
    }

    void UpdatePlayer(ulong clientID)
    {
        SetPlayerObject(clientID, ObjectManager.GetObjectById(GetPlayerObjectID(clientID)));
        SetPlayerCamera(clientID, GetPlayerObject(clientID)?.GetComponentInChildren<Camera>());
        SetPlayerTeam(clientID, TeamManager.GetTeam(clientID));
        Debug.Log($"{this.GetType()} : Updated player {clientID} infos, with object id {GetPlayerObjectID(clientID)} ({GetPlayerObject(clientID).name} - {GetPlayerCamera(clientID).name} - Team {GetPlayerTeam(clientID)})");
    }

    public static void SetPlayerObjectID(ulong clientId, int playerObjectId) { PlayerObjectIDs[(int) clientId] = playerObjectId; }
    void SetPlayerObject(ulong clientID, GameObject playerGo) { PlayerObjects[clientID] = playerGo; }
    void SetPlayerCamera(ulong clientID, Camera playerCamera) { PlayerCameras[clientID] = playerCamera; }
    void SetPlayerTeam(ulong clientID, int playerTeam) { PlayerTeams[clientID] = playerTeam; }

    public static int GetPlayerObjectID(ulong clientID) { return PlayerObjectIDs[(int) clientID]; }
    public static GameObject GetPlayerObject(ulong clientID) { return PlayerObjects[clientID]; }
    public static Camera GetPlayerCamera(ulong clientID) { return PlayerCameras[clientID]; }
    public static int GetPlayerTeam(ulong clientID) { return PlayerTeams[clientID]; }

}
