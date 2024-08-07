using UnityEngine;
using Unity.Netcode;

using scriptablesobjects;

public class piou : Projectile
{
    public ExplosionAsset explosion;

    public int ImpactDmg;
    public int ExplosionSize;
    public int ExpUnitDmg;
    public int ExpBuildingDmg;
    public float ExpDuration;
    public int outwardForce;

    public override void OnProjectileCollision(GameObject target)
    {
        if ((target.tag == "Player" ) && (target.GetComponent<NetworkObject>().IsOwner)) return;
        if ((target.tag == "Arme" ) && (target.GetComponent<NetworkObject>().IsOwner)) return;
        if ((target.tag == "Projectile" ) && (target.GetComponent<NetworkObject>().IsOwner)) return;

        //Debug.Log($"Piou impact {target} at {transform.position}");
        target.GetComponent<HealthSystem>()?.LoosePv(ImpactDmg);


        explosion.ExplodeAtPos(transform.position);
        SpawnManager.DestroyObject(gameObject);
        gameObject.SetActive(false);
    }

}


    
    
