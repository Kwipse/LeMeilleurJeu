using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncManager : NetworkBehaviour
{
    static SyncManager SM;
    void Awake()
    { 
        SM = this; 
        //Debug.Log("SyncManager : J'existe !");
    }


    public static void SyncHealth(GameObject patient, int amount)
    {
        SM.SetHealthRPC(patient, amount);
    }

    [Rpc(SendTo.NotMe, RequireOwnership = false)]
    void SetHealthRPC(NetworkObjectReference patientNo, int hp)
    {
        GameObject patient = patientNo;
        //patient.GetComponent<IHealth>().SetPv(hp);
        Debug.Log($"Health rpc : changing health value");
    }


}

