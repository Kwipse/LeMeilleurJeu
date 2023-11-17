using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

namespace AbstractClasses
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(ClientNetworkTransform))]
    //[RequireComponent(typeof(NetworkRigidbody))]

    public abstract class Projectile : NetworkBehaviour
    {
        public int Speed = 1000;

        Rigidbody rb;

        protected void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        public override void OnNetworkSpawn() 
        {
            ColorManager.SetObjectColors(gameObject);

            if (IsOwner)
                rb.AddForce(gameObject.transform.forward * Speed);
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

