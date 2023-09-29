using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;


public class UnitSystem : NetworkBehaviour
{
    public float attackRange = 1.0f;
    public float sightRange = 10.0f;

    public GameObject explosion;

    bool attackMode;

	GameObject target;
	Transform targetTransform;
    Vector3 destination;
	NavMeshAgent agent;

    Collider[] nearbyColliders;
    GameObject[] nearbyObjects;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        attackMode = true;
    }

    void Update()
    {
        if (attackMode) CheckForAttack();
    }

    void SwitchAttackMode()
    {
        attackMode = !attackMode;
    }

    void CheckForAttack()
    {
        target = FindClosestEnnemiInSightRange();
        if (target) { 
            MoveTowardEnnemi();
            Attack(); 
        }
    }

    void MoveTowardEnnemi()
    {
        targetTransform = target.transform; 
        MoveUnitToPosition(target.transform.position);

        bool isAttackPossible = attackRange >= Vector3.Distance(transform.position, targetTransform.position);
        if (isAttackPossible) 
            Attack();
    }
    


    void Attack()
    {
        SpawnManager.SpawnObjectByName(explosion.name,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }

    GameObject FindClosestEnnemiInSightRange()
    {
        GameObject closestEnnemi = null;
        float closestEnnemiDistance = sightRange;

        nearbyColliders = Physics.OverlapSphere(transform.position, sightRange);

        foreach(Collider col in nearbyColliders) 
        {
            GameObject go = col.gameObject;
            if (go == this.gameObject) break;
            if (go.tag == "ground") break;

            Debug.Log($"Mob collided with {go.name}");

            var nearbyObjectDistance = Vector3.Distance(go.transform.position, this.gameObject.transform.position);
            bool isCloser = nearbyObjectDistance < closestEnnemiDistance;
            bool isEnnemi = TeamManager.AreObjectsEnnemies(this.gameObject, go);

            if (isCloser && isEnnemi)
            {
                closestEnnemi = col.gameObject;
                closestEnnemiDistance = nearbyObjectDistance;
            }

        }

        return closestEnnemi;
    }

    void MoveUnitToPosition(Vector3 pos) 
    {
        agent.destination = pos; 
    }
}
