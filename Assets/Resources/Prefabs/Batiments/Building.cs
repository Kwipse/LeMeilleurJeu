using UnityEngine;
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

    public delegate void TriggerEvent(Collider col);
    public event TriggerEvent TriggerEnterEvent;
    public event TriggerEvent TriggerExitEvent;

    bool tmpKinematic;
    bool tmpTrigger;

    public virtual void Awake() 
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //col = gameObject.GetComponent<Collider>();

        tmpKinematic = rb.isKinematic;
        //tmpTrigger = col.isTrigger;

        //Set this for blueprint
        rb.isKinematic = true;

        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>())
        {
            if (c.GetType() == typeof(MeshCollider)) {
                c.GetComponent<MeshCollider>().convex = true;
                c.isTrigger = true; }
        }


        //col.isTrigger = true;
    }

    public override void StartAfterGameSync()
    {
        ColorManager.SetObjectColors(gameObject);

        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>())
        {
            if (c.GetType() == typeof(MeshCollider))
            {
                c.isTrigger = false;
                c.GetComponent<MeshCollider>().convex = false;
            }
        }

        //Go back to physics as set in editor
        rb.isKinematic = tmpKinematic;
        //col.isTrigger = tmpTrigger;

        if (!IsOwner) { this.enabled = false; }


        //Update the navmesh
        NavMeshSurface nms = GameObject.Find("Sol").GetComponent<NavMeshSurface>();
        nms.UpdateNavMesh(nms.navMeshData);
    }

    //Send events on triggers (for blueprints)
    void OnTriggerEnter(Collider col) { if (TriggerEnterEvent != null) TriggerEnterEvent(col); }
    void OnTriggerExit(Collider col) { if (TriggerExitEvent != null) TriggerExitEvent(col); }
}
