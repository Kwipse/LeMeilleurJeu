using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


    public class Lazer : NetworkBehaviour
    {
    [HideInInspector] public float LazerDuration;
    [HideInInspector] public int damageToUnit;
    [HideInInspector] public int damageToBuilding;
    [HideInInspector] public int outwardForce;
    bool setToDestroy;

    
    public override void OnNetworkSpawn() {
        if (!IsOwner) { enabled = false; } }
    
    void Start()
    {
        Invoke("EndLazer", LazerDuration);
        setToDestroy = false;
    }

    void OnTriggerEnter(Collider col)
	{
        if (!IsOwner) return;
        GameObject target = col.gameObject;
        string tag = target.tag;

        int dmg = 0;
        if (tag == "Player") dmg = damageToUnit;
        if (tag == "Unit") dmg = damageToUnit;
        if (tag == "Building") dmg = damageToBuilding;

        //Debug.Log($"Explosion : Outward force = {outwardForce}");
        if (outwardForce != 0) {
            PushTarget(target, outwardForce, col.ClosestPoint(transform.position)); }

        //Debug.Log($"Explosion dealt {dmg}pv to {target.name}");
        //target.GetComponent<IHealth>()?.LoosePv(dmg);
        target.GetComponent<HealthSystem>()?.LoosePv(dmg);
	}

    void PushTarget(GameObject target, int force, Vector3 expCenter)
    {
        //Debug.Log($"Explosion : Pushing {target.name}");
        if (target) PushTargetRPC(target, force, expCenter);
    }

    [Rpc(SendTo.Everyone)]
    void PushTargetRPC(NetworkObjectReference targetNor, int force, Vector3 expPosition)
    {
        GameObject target = targetNor;
        if (!target) { return; }
        if (!target.GetComponent<NetworkObject>().IsOwner) { return; }

        Vector3 targetCenter = target.GetComponent<Collider>().bounds.center;
        Vector3 pushDirection = targetCenter - expPosition;
        Vector3 push = Vector3.Normalize(pushDirection) * force;

        //Debug.Log($"Add {push} force to {target.name}");
        target.GetComponent<Rigidbody>()?.AddForce(push);
    }


    void EndLazer() {
        if (setToDestroy) return;
        setToDestroy = true;
        SpawnManager.DestroyObject(this.gameObject); }

    }

