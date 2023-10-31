using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class BlueprintSystem : NetworkBehaviour 
{
    Rigidbody rb;
    Collider col;

    Behaviour[] behaviours;
    public bool isBlueprintAllowed;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();
        behaviours = GetComponentsInChildren<Behaviour>();
    }


    // Le blueprint et les fonctions associees ne sont accessibles
    // que lorsque l'objet est instancié sur le client.
    // 
    // Quand l'objet est créé sur le reseau via le SpawnManager, 
    // ce script sera désactivé.
    //
    public override void OnNetworkSpawn()
    {
        ColorManager.SetObjectColors(gameObject);
        col.isTrigger = false;
        this.enabled = false;
    }

    void Start()
    {
        rb.isKinematic = true;
        col.isTrigger = true;

        foreach (Behaviour b in behaviours) 
            if (b!=this) { b.enabled = false; } 

        isBlueprintAllowed = true;
        ColorBlueprint(); 
    }


    void OnTriggerEnter(Collider col) {
        //if (IsSpawned) {return;}
        if (col.gameObject.tag == "ground") {return;}

        isBlueprintAllowed = false;
        ColorBlueprint(); }


    void OnTriggerExit(Collider col) {
        //if (IsSpawned) {return;}
        if (col.gameObject.tag == "ground") {return;}

        isBlueprintAllowed = true;
        ColorBlueprint(); }


    void ColorBlueprint() {
        ColorManager.SetBlueprintColor(gameObject, isBlueprintAllowed); }

}

