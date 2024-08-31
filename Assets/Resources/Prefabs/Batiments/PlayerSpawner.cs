using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : SyncedBehaviour, ISyncBeforeGame, IWaitForGameSync
{
    //quand construit se declare au teammanager
    //is available gerer ici
    bool isAvailable = true;

    public int currentTeam = 0;
    
    public override void InitializeBeforeSync()
    {
        if (IsOwner)
        {
            if (currentTeam == 0) {
                currentTeam = TeamManager.GetTeam(NetworkManager.LocalClientId); }
            TeamManager.AddSpawner(gameObject, currentTeam);
            //Debug.Log($"adding player spawner");
        }

    }

    public override void StartAfterGameSync()
    {
    }

    public void SwitchAvailability()
    {
        isAvailable = !isAvailable ; 
    }
}

