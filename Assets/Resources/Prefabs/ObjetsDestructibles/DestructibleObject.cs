using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DestructibleObject : SyncedBehaviour, IWaitForGameSync
{
    void Awake()
    {
        HealthSystem hs = GetComponent<HealthSystem>();
    }

    public override void StartAfterGameSync()
    {

    }

    public override void OnDestroy()
    {
        SpawnManager.SpawnObject("CaisseDetruite",transform.position, transform.rotation);
        base.OnDestroy();
    }


}
