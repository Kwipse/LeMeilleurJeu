using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Building_manager : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject mob;

    public void CreateMob()
    {
        GameObject go = Instantiate(mob,spawnPoint.position,spawnPoint.rotation);
        go.GetComponent<NetworkObject>().Spawn();
    }
}
