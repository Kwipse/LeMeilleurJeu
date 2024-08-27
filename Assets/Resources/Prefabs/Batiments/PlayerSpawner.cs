using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    //quand construit se declare au teammanager
    //is available gerer ici
    bool isAvailable = true;

    void Awake()
    {
        TeamManager.AddSpawner();
    }

    public void SwitchAvailability()
    {
        isAvailable = !isAvailable ; 
    }
}

