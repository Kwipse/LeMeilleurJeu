using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : SyncedBehaviour, IWaitForGameSync
{
    //quand construit se declare au teammanager
    //is available gerer ici
    bool isAvailable = true;

    int currentTeam;
    
    public override void StartAfterGameSync()
    {
        if (IsOwner)
        {
            currentTeam = TeamManager.GetTeam(NetworkManager.LocalClientId);
            TeamManager.AddSpawner(gameObject, currentTeam);
        }
    }

    public void SwitchAvailability()
    {
        isAvailable = !isAvailable ; 
    }
}

