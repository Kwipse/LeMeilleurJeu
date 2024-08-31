using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructedObject : MonoBehaviour
{
    void Awake()
    {
        foreach (Transform tr in gameObject.GetComponentsInChildren<Transform>(true))
        {
            MeshCollider col = tr.gameObject.AddComponent<MeshCollider>();
            col.convex = true;
            Rigidbody rb = tr.gameObject.AddComponent<Rigidbody>();
        }

        Invoke("SelfDestroy", 5);
    }

    void SelfDestroy()
    {
        SpawnManager.DestroyObject(gameObject);
    }
}
