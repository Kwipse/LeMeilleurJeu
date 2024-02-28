using UnityEngine;
using Unity.Netcode;
using systems;

public class Explosion : NetworkBehaviour
{
    [HideInInspector] public float ExplosionSize;
    [HideInInspector] public float ExplosionDuration;
    [HideInInspector] public int damageToUnit;
    [HideInInspector] public int damageToBuilding;
    [HideInInspector] public int outwardForce;

    bool setToDestroy;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            enabled = false;
    }

    void Start()
    {
        //Debug.Log($"Exploding at {transform.position}, size {ExplosionSize}");

        Invoke("EndExplosion", ExplosionDuration);
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

        target.GetComponent<HealthSystem>()?.LoosePv(dmg);
        //Debug.Log($"Explosion dealt {dmg}pv to {target.name}");

        if (outwardForce != 0)
            PushTarget(target, outwardForce, col.ClosestPoint(transform.position));
        
	}


    void PushTarget(GameObject target, int force, Vector3 expCenter)
    {
        if (target) PushTargetRPC(target, force, expCenter);
    }

    [Rpc(SendTo.Everyone)]
    void PushTargetRPC(NetworkObjectReference targetNor, int force, Vector3 expPosition)
    {
        GameObject target = targetNor;
        if (!target.GetComponent<NetworkObject>().IsOwner) { return; }

        Vector3 targetCenter = target.GetComponent<Collider>().bounds.center;
        Vector3 pushDirection = targetCenter - expPosition;
        Vector3 push = Vector3.Normalize(pushDirection) * force;

        //Debug.Log($"Add {push} force to {target.name}");

        target.GetComponent<Rigidbody>()?.AddForce(push);
    }


    void EndExplosion() {
        if (setToDestroy) return;
        setToDestroy = true;
        SpawnManager.DestroyObject(this.gameObject); }
}
