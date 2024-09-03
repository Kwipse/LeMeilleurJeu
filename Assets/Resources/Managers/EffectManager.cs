using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;

public class EffectManager : SyncedBehaviour
{
    static List<LineRenderer> LLR;

    private IEnumerator coroutine;

    //Enable global access to static functions as EffectManager.function()
    static EffectManager EM;
    void Awake()
    { 
        EM = this; 
        LLR = new List<LineRenderer>();
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


    public static LineRenderer LineEffect(Vector3 startPos, Vector3 endPos, float time, Color color, float size = 0.1f)
    {
        LineRenderer LR = GetLineRenderer();
        LR.SetPosition(0, startPos);
        LR.SetPosition(1, endPos);
        LR.material.color = color;
        LR.startWidth = size;
        LR.endWidth = size;
         
        IEnumerator coroutine = DestroyLine(LR, time);
        EM.StartCoroutine(coroutine);
        
        return LR;
    }


    static LineRenderer AddLineRenderer()
    {
       GameObject lineSpawner = new GameObject("LineSpawner");
       lineSpawner.transform.SetParent(EM.gameObject.transform);
       LineRenderer LR = lineSpawner.AddComponent<LineRenderer>();

       LLR.Add(LR);
       LR.enabled = true;
       return LR;
    }

    static LineRenderer GetLineRenderer()
    {
        if (LLR.Count == 0) { return AddLineRenderer(); } 

        foreach (LineRenderer lr in LLR) {
            if (!lr.enabled) {
                lr.enabled = true;
                return lr; } }

        return AddLineRenderer();
    }

    private static IEnumerator DestroyLine(LineRenderer lr, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        lr.enabled = false;
    }

}

