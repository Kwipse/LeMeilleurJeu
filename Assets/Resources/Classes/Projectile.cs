using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

namespace classes {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(ClientNetworkTransform))]
    //[RequireComponent(typeof(NetworkRigidbody))]

    public abstract class Projectile : NetworkBehaviour
    {
        [HideInInspector]
        public int initialForce;

        Rigidbody rb;

        public virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public override void OnNetworkSpawn() 
        {
            ColorManager.SetObjectColors(gameObject);

            if (IsOwner)
                rb.AddForce(gameObject.transform.forward * initialForce);
        }

        public virtual void Start()
        {
            gameObject.tag = "Projectile";
        }


        void OnCollisionEnter(Collision collision)
        {
            if (IsOwner)
                OnProjectileCollision(collision.gameObject);
        }


        //Fonctions a implementer dans la classe héritante
        public abstract void OnProjectileCollision(GameObject go);
    }
}

