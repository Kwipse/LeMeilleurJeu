using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class UnitSpawnerSystem : MonoBehaviour
{
    public List<GameObject> UnitesDisponibles;
    //public bool isSelected;

    Collider buildingCollider;
    List<Collider> unitColliders;

    Vector3 rallyPos;
    Vector3 spawnPos;
    Quaternion spawnRot;

    Camera cam;
    Ray ray;
    RaycastHit hit;
    
    void Awake()
    {
        buildingCollider = gameObject.GetComponent<Collider>();

        unitColliders = new List<Collider>();
        foreach (GameObject unit in UnitesDisponibles)
            unitColliders.Add(unit.GetComponent<Collider>());
    }

    void Start()
    {
       MoveRallyPoint(Vector3.zero);
    }


    public void MoveRallyPoint(Vector3 pos) {
        rallyPos = pos; }


    public void SpawnUnit(int unitIndex)
    {
        Vector3 spawnerPos = gameObject.transform.position;
        Vector3 rallyDirection = Vector3.Normalize(rallyPos - spawnerPos);
        Vector3 offset = (unitColliders[unitIndex].bounds.size + buildingCollider.bounds.size)/2;

        spawnPos = spawnerPos + (rallyDirection * offset.magnitude);
        spawnPos.y = unitColliders[unitIndex].bounds.size.y;

        spawnRot = Quaternion.identity;

        Debug.Log($"{gameObject.name} : Production de {UnitesDisponibles[unitIndex].name} en {spawnPos}");
        //SpawnManager.SpawnObject(unitToSpawn, spawnPos, Quaternion.identity);

        SpawnManager.SpawnUnit(UnitesDisponibles[unitIndex], spawnPos, spawnRot, rallyPos);
    }
}
