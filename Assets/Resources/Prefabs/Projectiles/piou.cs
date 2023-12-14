using UnityEngine;
using Unity.Netcode;
using classes;
using systems;

public class piou : Projectile
{
    public int ImpactDmg;
    public int ExplosionSize;
    public int ExpUnitDmg;
    public int ExpBuildingDmg;
    public float ExpDuration;
    public int outwardForce;

    public override void OnProjectileCollision(GameObject target)
    {
        if ((target.tag == "Player" ) && (target.GetComponent<NetworkObject>().IsOwner))
            return;
        if ((target.tag == "Projectile" ) && (target.GetComponent<NetworkObject>().IsOwner))
            return;

        //Debug.Log($"Piou impact at {transform.position}");

        target.GetComponent<HealthSystem>()?.LoosePv(ImpactDmg);

        SpawnManager.SpawnExplosion(transform.position, ExplosionSize, ExpUnitDmg, ExpBuildingDmg, ExpDuration, outwardForce);
        SpawnManager.DestroyObject(gameObject);
        gameObject.SetActive(false);
    }

}


    
    
