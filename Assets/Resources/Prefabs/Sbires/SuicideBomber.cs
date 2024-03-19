using UnityEngine;
using classes;
using systems;
using scriptablesobjects;

public class SuicideBomber : Unit
{
    public ExplosionAsset explosion;
    HealthSystem health;

    public override void Awake()
    {
        health = GetComponent<HealthSystem>();
        base.Awake();
    } 

    public override void AttackAction() 
    {
        health.LoosePv(100);
        explosion.ExplodeAtPos(transform.position);
        gameObject.SetActive(false);
    }
}
