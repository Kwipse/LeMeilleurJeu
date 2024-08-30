using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PhysicsManager : SyncedBehaviour
{
    //Enable global access to static functions as SpawnManager.function()
    static PhysicsManager PM;
    void Awake()
    { 
        PM = this; 
    }


    public static void AddForce(GameObject go, Vector3 force)
    {
        bool isOwner = go.transform.root.GetComponent<NetworkObject>().IsOwner;
        go.GetComponent<Rigidbody>()?.AddForce(force);
        if (!isOwner)
        {
            if (!go.GetComponent<NetworkObject>()) { return; }
            PM.AddForceRpc(go, force);
            return;
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    void AddForceRpc(NetworkObjectReference nor, Vector3 force)
    {
        GameObject go = nor;
        if (go.GetComponent<NetworkObject>().IsOwner)
        {
            AddForce(go, force);
        }

    }

}

