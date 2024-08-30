using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistOfFury : Arme
{
    public override void Awake()
    {
        DeactivateCollider();
        base.Awake();
    }
      
    public override void OnShoot()
    {
        ActivateCollider();
        Animation anim = gameObject.GetComponentInChildren<Animation>();
        anim.Play();
        Invoke("DeactivateCollider", 0.5f);

    }
    
    void ActivateCollider()
    {
        Collider col = gameObject.GetComponentInChildren<Collider>();
        col.enabled = true;
    }

    void DeactivateCollider()
    {
        Collider col = gameObject.GetComponentInChildren<Collider>();
        col.enabled = false;

    }

    void OnTriggerEnter(Collider col)
    {
        if (!IsOwner) { return; }
        Debug.Log($"{col.gameObject}");
        Debug.Log($"{GetWeaponSystem().gameObject}");
        if (col.gameObject == GetWeaponSystem().gameObject) { return; }
        Debug.Log($"Fist of fury : trigger on {col.gameObject.name}");
        col.gameObject.GetComponent<HealthSystem>()?.LoosePv(25);

        Vector3 force = col.transform.position - transform.position;
        PhysicsManager.AddForce(col.gameObject, force * 300);
    }
}

