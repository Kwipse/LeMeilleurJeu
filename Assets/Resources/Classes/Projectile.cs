using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using systems;
using classes;

namespace classes {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(ClientNetworkTransform))]

    public abstract class Projectile : NetworkBehaviour
    {
        [HideInInspector]
        public GameObject weapon, weaponHolder;
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



        public void SetWeapon(GameObject weaponToSet) {
            weapon = weaponToSet;
            weaponHolder = weaponToSet.GetComponent<Arme>().GetWeaponSystem().gameObject;
        }


        void OnCollisionEnter(Collision col)
        {
            //Don't hit owner
            if (col.gameObject == weapon) { return; }
            if (col.gameObject == weaponHolder) { return; }
            //if (col.gameObject.tag == "Projectile") { return; }

            //Debug.Log($"{gameObject.name} collided with {col.gameObject.name}");
            OnProjectileCollision(col.gameObject);
        }


        //Fonctions a implementer dans la classe héritante
        public abstract void OnProjectileCollision(GameObject go);



    }
}

