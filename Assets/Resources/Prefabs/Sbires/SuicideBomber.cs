using UnityEngine;
using classes;
using systems;

public class SuicideBomber : Unit
{
    HealthSystem health;

    public override void Awake()
    {
        health = GetComponent<HealthSystem>();
        base.Awake();
    } 

    public override void AttackAction() 
    {
        health.LoosePv(100);
        SpawnManager.SpawnObjectByName("Explosion",transform.position,Quaternion.identity); 
        gameObject.SetActive(false);
    }
}
