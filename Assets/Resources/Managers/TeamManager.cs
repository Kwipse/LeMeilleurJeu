using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TeamManager : SyncedBehaviour, ISyncBeforeGame 
{
    static NetworkList<int> ClientTeam;	

    static TeamManager TM;
    void Awake() 
    { 
        TM = this;
        //Debug.Log("TeamManager : J'existe !");

        ClientTeam = new NetworkList<int>();
    }

    public override void InitializeBeforeSync()
    {
        ClientTeam.OnListChanged += OnClientTeamChanged;

        if (IsServer)
            InitializeClientTeam();

        if (IsClient) {
            //Debug.Log($"ColorManager : Team set to {ClientTeam[(int) clientId]}");
        }
    }




    void InitializeClientTeam()
    {
        while (ClientTeam.Count < 10) { ClientTeam.Add(ClientTeam.Count); }
    }

    void OnClientTeamChanged(NetworkListEvent<int> ListEvent) {
        ColorManager.SetPlayerColors((ulong) ListEvent.Index); }


    void AddNewClient() {
        TM.AddClientServerRPC(); }

    [ServerRpc(RequireOwnership = false)]
    void AddClientServerRPC(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        ClientTeam.Add((int) clientId);
        Debug.Log($"TeamManager : Client {clientId} added to team {clientId}");
    }

    public static void SetTeam(ulong clientId, int teamId) 
    { TM.SetTeamServerRPC(clientId, teamId); }
    [ServerRpc(RequireOwnership = false)]
    void SetTeamServerRPC(ulong clientId, int teamId)
    {
        ClientTeam[(int) clientId] = teamId; 
        Debug.Log($"TeamManager : Player {clientId} team set to {teamId}");
    }

    public static int GetTeam(ulong clientId) 
    {
        return ClientTeam.Count != 0 ? ClientTeam[(int) clientId] : 0 ; 
    }

    public static bool AreObjectsEnnemies(GameObject go1, GameObject go2)
    {
        int team1 = ClientTeam[(int) go1.GetComponent<NetworkObject>().OwnerClientId];
        int team2 = ClientTeam[(int) go2.GetComponent<NetworkObject>().OwnerClientId];

        if (team1 != team2)
            return true;

        return false;
    }



    public override void OnNetworkDespawn()
    {
        ClientTeam.OnListChanged -= OnClientTeamChanged;
    }
}
