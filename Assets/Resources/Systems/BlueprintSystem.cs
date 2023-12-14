using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

namespace systems {

    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(ClientNetworkTransform))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]

    public class BlueprintSystem : NetworkBehaviour 
    {
        Behaviour[] behaviours;
        Rigidbody rb;
        Collider col;

        public bool isBlueprintAllowed;

        // Le blueprint et les fonctions associees ne sont accessibles
        // que lorsque l'objet est instancié sur le client.
        // 
        // Quand l'objet est créé sur le reseau via le SpawnManager, 
        // ce script sera désactivé.
        //
        //
        void Awake()
        {
            behaviours = GetComponentsInChildren<Behaviour>();
            rb = gameObject.GetComponent<Rigidbody>();
            col = gameObject.GetComponent<Collider>();
        }

        public override void OnNetworkSpawn()
        {
            this.enabled = false;  
        }

        void Start()
        {
            //Desactive les autres fonctionnalités de l'objet
            foreach (Behaviour b in behaviours) 
                if (b!=this) { b.enabled = false; } 

            //Initialise la physique et la collision du blueprint
            rb.isKinematic = true;
            col.isTrigger = true;

            //Initialise le blueprint
            isBlueprintAllowed = true;
            ColorBlueprint(); 
        }


        void OnTriggerEnter(Collider col) {
            if (IsSpawned) {return;}
            if (col.gameObject.tag == "ground") {return;}

            isBlueprintAllowed = false;
            ColorBlueprint(); }


        void OnTriggerExit(Collider col) {
            if (IsSpawned) {return;}
            if (col.gameObject.tag == "ground") {return;}

            isBlueprintAllowed = true;
            ColorBlueprint(); }


        void ColorBlueprint() {
            ColorManager.SetBlueprintColor(gameObject, isBlueprintAllowed); }

    }

}
