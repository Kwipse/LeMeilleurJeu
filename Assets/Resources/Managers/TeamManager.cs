using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TeamManager : SyncedBehaviour, ISyncBeforeGame 
{
    static NetworkList<int> ClientTeam;	

    static Dictionary<GameObject, int> spawners;



    static TeamManager TM;
    void Awake() 
    { 
        TM = this;
        ClientTeam = new NetworkList<int>();
        spawners = new Dictionary<GameObject, int>();
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


    //SPAWNERS
    public static void AddSpawner(GameObject spawner, int team)
    {
        spawners.Add(spawner, team);
        ShowSpawnerList();
    }
    public static void RemoveSpawner(GameObject spawner)
    {
        spawners.Remove(spawner);
    }
    public static void SetSpawnerTeam(GameObject spawner, int team)
    {
        spawners[spawner] = team;
    }

    public static void ShowSpawnerList()
    {
        foreach (var spawner in spawners)
        {
            Debug.Log($"{spawner.Key.name}/Team{spawner.Value}");
        }
    }


    public static List<GameObject> GetPlayerSpawners(ulong clientId)
    {
        int clientTeam = GetTeam(clientId);
        List<GameObject> playerSpawners = new List<GameObject>();

        foreach (var spawner in spawners)
        {
            if (spawner.Value == clientTeam) {
                playerSpawners.Add(spawner.Key); }
        }

        return playerSpawners;
    }



    public override void OnNetworkDespawn()
    {
        ClientTeam.OnListChanged -= OnClientTeamChanged;
    }
}
