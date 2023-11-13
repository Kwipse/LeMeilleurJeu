using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Explosion : NetworkBehaviour
{
    [HideInInspector] public float ExplosionSize;
    [HideInInspector] public float ExplosionDuration;
    [HideInInspector] public int damageToUnit;
    [HideInInspector] public int damageToBuilding;

    bool setToDestroy;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            enabled = false;
    }

    void Start()
    {
        Debug.Log($"Exploding at {transform.position}, size {ExplosionSize}");

        

        Invoke("EndExplosion", ExplosionDuration);
        setToDestroy = false;
    }

	void OnCollisionEnter(Collision collision)
	{
        int dmg = 0;
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
