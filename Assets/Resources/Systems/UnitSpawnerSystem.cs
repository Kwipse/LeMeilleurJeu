using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class UnitSpawnerSystem : MonoBehaviour
{
    public GameObject unitToSpawn;
    //public bool isSelected;

    Vector3 rallyPos;
    Vector3 spawnPos;
    Quaternion spawnRot;
    Collider unitCollider;
    Collider buildingCollider;

    Camera cam;
    Ray ray;
    RaycastHit hit;
    
    void Awake()
    {
        unitCollider = unitToSpawn.GetComponent<Collider>();
        buildingCollider = gameObject.GetComponent<Collider>();
    }

    void Start()
    {
       MoveRallyPoint(Vector3.zero);
    }


    public void MoveRallyPoint(Vector3 pos) {
        rallyPos = pos; }


    public void SpawnUnit()
    {
        Vector3 spawnerPos = gameObject.transform.position;
        Vector3 rallyDirection = Vector3.Normalize(rallyPos - spawnerPos);
        Vector3 offset = (unitCollider.bounds.size + buildingCollider.bounds.size)/2;

        spawnPos = spawnerPos + (rallyDirection * offset.magnitude);
        spawnPos.y = unitCollider.bounds.size.y;

        spawnRot = Quaternion.identity;

        Debug.Log($"{gameObject.name} : Production de {unitToSpawn.name} en {spawnPos}");
        //SpawnManager.SpawnObject(unitToSpawn, spawnPos, Quaternion.identity);

        SpawnManager.SpawnUnit(unitToSpawn, spawnPos, spawnRot, rallyPos);
    }
}
