using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using systems;

namespace classes {

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(BlueprintSystem))]

    public class Building : NetworkBehaviour
    {
        Rigidbody rb;
        Collider col;

        public virtual void Awake() 
        {
            rb = gameObject.GetComponent<Rigidbody>();
            col = gameObject.GetComponent<Collider>();
        }

        public override void OnNetworkSpawn()
        {
            ColorManager.SetObjectColors(gameObject);

            rb.isKinematic = false;
            col.isTrigger = false;

            if (!IsOwner)
                this.enabled = false;
        }
        
    }
}
