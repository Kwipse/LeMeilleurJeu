using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using managers;


namespace classes {

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

    public class Building : NetworkBehaviour
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
            col = gameObject.GetComponent<Collider>();

            tmpKinematic = rb.isKinematic;
            tmpTrigger = col.isTrigger;

            //Set this for blueprint
            rb.isKinematic = true;
            col.isTrigger = true;
        }

        public override void OnNetworkSpawn()
        {
            ColorManager.SetObjectColors(gameObject);

            //Go back to physics as set in editor
            rb.isKinematic = tmpKinematic;
            col.isTrigger = tmpTrigger;

            if (!IsOwner)
                this.enabled = false;
        }

        //Send events on triggers (for blueprints)
        void OnTriggerEnter(Collider col) { if (TriggerEnterEvent != null) TriggerEnterEvent(col); }
        void OnTriggerExit(Collider col) { if (TriggerExitEvent != null) TriggerExitEvent(col); }
    }
}
