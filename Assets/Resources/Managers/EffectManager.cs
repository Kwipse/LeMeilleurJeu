using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class EffectManager : SyncedBehaviour
{
    //Enable global access to static functions as EffectManager.function()
    static EffectManager EM;
    void Awake()
    { 
        EM = this; 
    }


    public static void SpawnTrace(string prefabName, RaycastHit hit) {
        SpawnManager.SpawnObject(prefabName, hit.point, Quaternion.LookRotation(hit.normal)); }

    public static void SpawnTrace(GameObject prefab, RaycastHit hit) {
        SpawnManager.SpawnObject(prefab, hit.point, Quaternion.LookRotation(hit.normal)); }

    public static void SpawnRagdoll(GameObject go, GameObject ragdoll) {
        SpawnManager.SpawnObject(ragdoll, go.transform.position, go.transform.rotation); }

    public static void SpawnRagdoll(GameObject go, string ragdollName) {
        SpawnManager.SpawnObject(ragdollName, go.transform.position, go.transform.rotation); 
    }

    public static void ReplaceDestructibleObject(GameObject go, GameObject destroyedGo) {
        SpawnManager.SpawnObject(destroyedGo, go.transform.position, go.transform.rotation); }

    public static void ReplaceDestructibleObject(GameObject go, string destroyedGoName) {
        SpawnManager.SpawnObject(destroyedGoName, go.transform.position, go.transform.rotation); }


}

