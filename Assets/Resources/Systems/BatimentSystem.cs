using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BatimentSystem : NetworkBehaviour 
{
    public Material mBlueprintAllowed;
    public Material mBlueprintNotAllowed;

    Renderer[] BatRenderers;
    Behaviour[] BatBehaviours;

    public bool isBlueprintAllowed;

    void Awake()
    {
        BatRenderers = GetComponentsInChildren<Renderer>(); 
        BatBehaviours = GetComponentsInChildren<Behaviour>();
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void OnNetworkSpawn()
    {
        ColorManager.SetObjectColors(gameObject);

        gameObject.GetComponent<Collider>().isTrigger = false;

        this.enabled = false;
    }

    void Start()
    {
        gameObject.GetComponent<Collider>().isTrigger = true;
        foreach (Behaviour b in BatBehaviours) { if (b!=this) { b.enabled = false; } }
        foreach (Renderer r in BatRenderers) { r.material = mBlueprintAllowed; } 
        isBlueprintAllowed = true;
    }


    void OnTriggerEnter(Collider col)
    {
        if (IsSpawned) {return;}

        if (col.gameObject.CompareTag("ground")) {return;}
        foreach (Renderer r in BatRenderers) { r.material = mBlueprintNotAllowed; } 
        isBlueprintAllowed = false;
    }

    void OnTriggerExit(Collider col)
    {
        if (IsSpawned) {return;}

        foreach (Renderer r in BatRenderers) { r.material = mBlueprintAllowed; } 
        isBlueprintAllowed = true;
    }

}
