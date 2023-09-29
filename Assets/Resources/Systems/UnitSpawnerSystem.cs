using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnerSystem : MonoBehaviour
{

    public GameObject unitToSpawn;
    //public bool isSelected;

    Vector3 spawnPos;
    Vector3 rallyPointPos;
    Vector3 rallyDirection;
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

    // Start is called before the first frame update
    void Start()
    {
       MoveRallyPoint(Vector3.zero);
       SpawnUnit();
    }

    public void MoveRallyPoint(Vector3 pos)
    {
        rallyPointPos = pos;
        Vector3 rallyDirection = rallyPointPos - gameObject.transform.position;
        rallyDirection.Normalize();
        Vector3 offset = (unitCollider.bounds.size + buildingCollider.bounds.size)/2;
        
        spawnPos = gameObject.transform.position + (rallyDirection * offset.magnitude);
    }


    public void SpawnUnit()
    {
        SpawnManager.SpawnObject(unitToSpawn, spawnPos, Quaternion.identity);
    }
}
