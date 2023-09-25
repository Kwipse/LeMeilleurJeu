using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class TeamManager : NetworkBehaviour
{
	static NetworkList<int> ClientTeam;	
	
	static TeamManager TM;
	void Awake() 
	{ 
        TM = this;
        Debug.Log("TeamManager : J'existe !");

        ClientTeam = new NetworkList<int>();
    }
	
    public override void OnNetworkSpawn()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;

        ClientTeam.OnListChanged += OnClientTeamChanged;

        //AddNewClient();
        if (IsServer)
        {
            InitializeClientTeam();
        }
        if (IsClient)
        {
            Debug.Log($"ColorManager : Team set to {ClientTeam[(int) clientId]}");
        }

    }

    void OnClientTeamChanged(NetworkListEvent<int> ListEvent)
    {
        ColorManager.SetPlayerColors((ulong) ListEvent.Index);
    }

    void InitializeClientTeam()
    {
        ClientTeam.Add(0);
        ClientTeam.Add(1);
        ClientTeam.Add(2);
        ClientTeam.Add(3);
        ClientTeam.Add(4);
        ClientTeam.Add(5);
    }

    void AddNewClient()
        { TM.AddClientServerRPC(); }
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
        return ClientTeam[(int) clientId]; 
    }
  
}
