using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

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
        Collider col;

        public virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            col.enabled = false;
        }

        public override void OnNetworkSpawn() 
        {
            ColorManager.SetObjectColors(gameObject);
        }

        public virtual void Start()
        {
            gameObject.tag = "Projectile";
        }


        //Callback from SpawnManager
        public void SetWeapon(GameObject weaponToSet) {
            weapon = weaponToSet;
            weaponHolder = weapon.GetComponent<Arme>().GetWeaponSystem().gameObject;
            FireProjectile();
        }

        void FireProjectile()
        {
            col.enabled = true;
            rb.AddForce(gameObject.transform.forward * initialForce);
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

