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
    }

    public override void OnNetworkSpawn() 
    {
    }


    //Callback from SpawnManager
    public void FireProjectile(GameObject firingWeapon)
    {
        gameObject.SetActive(true);
        weapon = firingWeapon;
        weaponHolder = PlayerManager.GetPlayerObject(gameObject.GetComponent<NetworkObject>().OwnerClientId);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.enabled = false;
        rb.isKinematic = true;

        ColorManager.SetObjectColors(gameObject);


        //Debug.Log($"Firing {gameObject.name}/{gameObject.GetComponent<NetworkObject>().OwnerClientId}");
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(gameObject.transform.forward * initialForce); //Todo : initialForce devrait venir de l'arme, et pas du projectile
        col.enabled = true;
    }



    void OnCollisionEnter(Collision col)
    {
        //Debug.Log($"{NetworkManager.LocalClientId}/{gameObject.GetComponent<NetworkObject>().OwnerClientId} collision");
        if (NetworkManager.LocalClientId != gameObject.GetComponent<NetworkObject>().OwnerClientId) { return; }

        //Don't hit owner
        if (col.gameObject == weapon) { return; }
        if (col.gameObject == weaponHolder) { return; }
        //if (col.gameObject.tag == "Projectile") { return; }

        //Debug.Log($"{gameObject.name}/{gameObject.GetComponent<NetworkObject>().OwnerClientId} of {weaponHolder.name}/{weaponHolder.GetComponent<NetworkObject>().OwnerClientId} collided with {col.gameObject.name}/{col.gameObject.GetComponent<NetworkObject>().OwnerClientId}");
        OnProjectileCollision(col.gameObject);
    }


    //Fonctions a implementer dans la classe héritante
    public abstract void OnProjectileCollision(GameObject go);



}

