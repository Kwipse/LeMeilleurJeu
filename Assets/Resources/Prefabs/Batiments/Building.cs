using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine.AI;
using Unity.AI.Navigation;


//[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]

public class Building : SyncedBehaviour, IWaitForGameSync
{
    Rigidbody rb;
    Collider col;

    NavMeshSurface nms;
    NavMeshModifier nmm;
    NavMeshLinks_AutoPlacer nmlinker;

    public delegate void TriggerEvent(Collider col);
    public event TriggerEvent TriggerEnterEvent;
    public event TriggerEvent TriggerExitEvent;

    bool tmpKinematic;
    bool tmpTrigger;

    public virtual void Awake() 
    {
        rb = gameObject.GetComponent<Rigidbody>();
        nms = GameObject.Find("Sol").GetComponent<NavMeshSurface>();
        nmlinker = GameObject.Find("Sol").GetComponent<NavMeshLinks_AutoPlacer>();
        nmm = gameObject.AddComponent<NavMeshModifier>();

        //nmm.overrideGenerateLinks = true;
        //nmm.generateLinks = true;
        
        tmpKinematic = rb.isKinematic;

        //Set this for blueprint
        rb.isKinematic = true;

        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>())
        {
            if (c.GetType() == typeof(MeshCollider)) {
                c.GetComponent<MeshCollider>().convex = true;
                c.isTrigger = true; }
        }
    }

    public override void StartAfterGameSync()
    {
        ColorManager.SetObjectColors(gameObject);

        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>())
        {
            if (c.GetType() == typeof(MeshCollider)) {
                c.isTrigger = false;
                c.GetComponent<MeshCollider>().convex = false; }
        }

        //Go back to physics as set in editor
        rb.isKinematic = tmpKinematic;


        //Update the navmesh
        NavmeshManager.UpdateNavmesh();

        Invoke("UpdateLinks",1);
    }

    //Send events on triggers (for blueprints)
    void OnTriggerEnter(Collider col) { if (TriggerEnterEvent != null) TriggerEnterEvent(col); }
    void OnTriggerExit(Collider col) { if (TriggerExitEvent != null) TriggerExitEvent(col); }

    public override void OnNetworkDespawn()
    {
        nmm.ignoreFromBuild = true;

        //Destroy(nmm);
        NavmeshManager.UpdateNavmesh();
        Invoke("UpdateLinks",1);

    }

    void UpdateLinks()
    {
        //GameObject.Find("Sol").GetComponent<NavMeshLinks_AutoPlacer>().Generate();
    }
}

