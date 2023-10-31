using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Rigidbody))]

public class UnitSystem : NetworkBehaviour
{
    public float attackRange = 2.0f;
    public float attackCooldown = 1.0f;
    public float sightRange = 50.0f;
    public float unitSpeed = 15.0f;
    public float unitAngularSpeed = 1000.0f;
    public float unitAcceleration = 1000.0f;

    public GameObject explosion;

    NavMeshAgent agent;
    HealthSystem health;

    List<Vector3> moveWaypoints;
    Collider[] nearbyColliders;
    GameObject[] nearbyObjects;
    GameObject ennemiTarget;
    bool attackMode;
    //bool isAttacking;
    //

    void Awake() 
    {
        health = gameObject.GetComponent<HealthSystem>();
        agent = GetComponent<NavMeshAgent>();

        moveWaypoints = new List<Vector3>();
    }

    public override void OnNetworkSpawn()
    {
        ColorManager.SetObjectColors(gameObject);

        if (!IsOwner)
            this.enabled = false;
    }

    void Start()
    {
        //moveWaypoints.Add(gameObject.transform.position);
        agent.speed = unitSpeed;
        agent.angularSpeed = unitAngularSpeed;
        agent.acceleration = unitAcceleration;

    }

    void Update()
    {
        if (!ennemiTarget) 
            MoveToDestination(); 
            
        if (!attackMode)
            return;

        //Debug.Log($"Agent is searching for a target");

        if (!ennemiTarget) {
            //Debug.Log($"{gameObject.name} has no current target");
            ennemiTarget = FindClosestEnnemiInSightRange(); }

        if (ennemiTarget) {
            //Debug.Log($"{gameObject.name} is targetting {ennemiTarget.name}");
            AttackEnnemiTarget(ennemiTarget); }
    }


    void MoveToDestination() 
    {
        //Reactualise la destination si elle a changee
        if (Vector3.SqrMagnitude(agent.destination - moveWaypoints[0]) > 1 ) {
            agent.destination = moveWaypoints[0];
            //Debug.Log($"Agent moving to new waypoint : {agent.destination}");
            agent.isStopped = false; }
            
        //Si l'agent arrive au Waypoint
        if (Vector3.SqrMagnitude(gameObject.transform.position - agent.destination) > 1 )
            return;

        //Si il reste des Waypoints a parcourir
        if (moveWaypoints.Count > 1) {
            //Debug.Log($"Agent got to waypoint");
            moveWaypoints.Remove(moveWaypoints[0]);
            return; }

        //Stopper l'agent quand il est arrivé
        if (!agent.isStopped) {
            //Debug.Log($"Agent reached destination");
            attackMode = true; 
            agent.isStopped = true; }
    }


    void AttackEnnemiTarget(GameObject targetGo) {

        agent.destination = targetGo.transform.position;
        agent.isStopped = false;

        //Attaque si l'ennemi est a portée 
        //On utilise SqrMagnitude pour eviter de calculer des racines
        float sqrTargetSize = Vector3.SqrMagnitude(targetGo.GetComponent<Collider>().bounds.size);
        float sqrDistanceToTarget = Vector3.SqrMagnitude(this.gameObject.transform.position - targetGo.transform.position);
        float sqrAttackRange = attackRange * attackRange;

        //Debug.Log($"Agent distance to target is {sqrDistanceToTarget} and target size is {sqrTargetSize}");
        bool isTargetInRange = sqrAttackRange > (sqrDistanceToTarget - sqrTargetSize);

        if (isTargetInRange) {
            Debug.Log($"Agent {gameObject.name} attacks the {targetGo.name}");
            AttackAction(); } 
    }


    void AttackAction() 
    {
        health.LoosePv(100);
        SpawnManager.SpawnObjectByName(explosion.name,transform.position,Quaternion.identity); 
        gameObject.SetActive(false);
    }


    public void MoveUnitToPos(Vector3 pos, bool attackMove) {
        if (!Input.GetKey(KeyCode.LeftShift)) moveWaypoints.Clear();
        moveWaypoints.Add(pos);
        ennemiTarget = null;
        attackMode = false; }


    GameObject FindClosestEnnemiInSightRange()
    {
        GameObject closestEnnemi = null;
        float closestEnnemiDistance = sightRange;

        nearbyColliders = Physics.OverlapSphere(transform.position, sightRange);
        //Debug.Log($"Agent found {nearbyColliders.Length} colliders in range");

        foreach(Collider col in nearbyColliders) 
        {
            GameObject go = col.gameObject;

            //Ne considerer que les autres unités & buildings
            if (!go) continue;
            if (go == this.gameObject) continue;
            if (go.tag != "Unit" && go.tag != "Player" && go.tag != "Building" ) continue;

            //Ne considerer que les ennemis
            bool isEnnemi = TeamManager.AreObjectsEnnemies(this.gameObject, go);
            if (!isEnnemi) continue;

            //Debug.Log($"Agent {this.name} found ennemy {go.name}");

            //Determiner l'ennemi le plus proche
            var nearbyObjectDistance = Vector3.SqrMagnitude(go.transform.position - this.gameObject.transform.position);
            bool isCloser = nearbyObjectDistance < closestEnnemiDistance * closestEnnemiDistance;
            if (!isCloser) continue;

            closestEnnemi = col.gameObject;
            closestEnnemiDistance = nearbyObjectDistance; 
        }

        //if (closestEnnemi)
        //    Debug.Log($"Agent {this.name} closest ennemy is {closestEnnemi.name}");

        return closestEnnemi;
    }

}
