using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(SphereCollider))]

public class ExplosionScript : NetworkBehaviour
{
    public int damageToUnit = 25;
    public int damageToBuilding = 25;

    int dmg;
    bool setToDestroy;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            enabled = false;
    }

    void Start()
    {
        setToDestroy = false;
        Invoke("EndExplosion", 0.25f);
    }

	void OnCollisionEnter(Collision collision)
	{
        if (!IsOwner) return;

        switch(collision.gameObject.tag) 
        {
            case "Player" or "Unit":
                dmg = damageToUnit;
                break;

            case "Building":
                dmg = damageToBuilding;
                break;

            default:
                break; }

        collision.collider.GetComponent<HealthSystem>()?.LoosePv(dmg);
	}


    void EndExplosion() {
        if (setToDestroy) return;
        setToDestroy = true;
        SpawnManager.DestroyObject(this.gameObject); }
}
