using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        ColorManager.SetObjectColors(gameObject);
        if (IsOwner)
        {
            Invoke("SelfDestroy", 15);
        }
    }
    void SelfDestroy()
    {
        SpawnManager.DestroyObject(gameObject);
    }
}
