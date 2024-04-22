using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using managers;

namespace systems {

    [RequireComponent(typeof(Collider))]

    public class UnitSpawnerSystem : NetworkBehaviour
    {
        public List<GameObject> AvailableUnits;
        List<GameObject> AvailableSpawners;

        public bool isMatchUnitToSpawner = true;
        
        GameObject unitSpawner;
        Collider spawnerCollider;

        Vector3 rallyPos;
        Vector3 spawnPos;
        Quaternion spawnRot;

        Camera cam;
        Ray ray;
        RaycastHit hit;

        void Awake()
        {
            unitSpawner = gameObject;
        }

        public override void OnNetworkSpawn() {
            if (!IsOwner) {enabled = false;} }

        void Start()
        {
            InitRallyPoint();
            InitSpawner();
        }


        //Available Units
        public void AddAvailableUnit(GameObject unitPrefab) {
            AvailableUnits.Add(unitPrefab); }

        public void RemoveAvailableUnit(GameObject unitPrefab) {
            AvailableUnits.Remove(unitPrefab); }

        public void ClearAvailableUnitList() {
            AvailableUnits.Clear(); }



        //Available Spawners
        void InitSpawner()
        {
            //Populate AvailableSpawners 
            AvailableSpawners = new List<GameObject>();
            foreach (Transform tr in GetComponentsInChildren<Transform>()) {
                if (tr.gameObject.tag == "UnitSpawner") {
                    AvailableSpawners.Add(tr.gameObject); } }

            //If at least one spawner found, end init 
            if (AvailableSpawners.Count != 0) return;

            //Default spawner : 
            Vector3 spawnerSize = unitSpawner.GetComponent<Collider>().bounds.size;
            Vector3 unitSize = AvailableUnits[0].GetComponent<Collider>().bounds.size;
            Vector3 offset = (spawnerSize + unitSize)/2;
            Vector3 rallyDirection = Vector3.Normalize(rallyPos - unitSpawner.transform.position);

            GameObject defaultSpawner = new GameObject();
            defaultSpawner.name = "DefaultSpawner";
            defaultSpawner.tag = "UnitSpawner";
            defaultSpawner.transform.parent = gameObject.transform;
            defaultSpawner.transform.position = gameObject.transform.position;
            defaultSpawner.transform.position += (rallyDirection * offset.magnitude);

            defaultSpawner.transform.position += new Vector3(0, unitSize.y, 0);
            defaultSpawner.transform.rotation = Quaternion.identity;

            AvailableSpawners.Add(defaultSpawner);
        }

        public void AddAvailableSpawner(GameObject spawnerObject) {
            AvailableSpawners.Add(spawnerObject); }

        public void RemoveAvailableSpawner(GameObject spawnerObject) {
            AvailableSpawners.Remove(spawnerObject); }

        public void ClearAvailableSpawnerList() {
            AvailableSpawners.Clear(); }



        //Rally Points
        void InitRallyPoint() {
            MoveRallyPoint(Vector3.zero); }

        public void MoveRallyPoint(Vector3 pos) {
            rallyPos = pos; }

        public Vector3 GetRallyPoint() {
            return rallyPos; }



        //Unit Spawning
        public void SpawnUnit(GameObject unitPrefab, GameObject specificSpawner = null) 
        {
            if (!AvailableUnits.Contains(unitPrefab)) {
                Debug.Log($"Tried to spawn unit prefab \"{unitPrefab.name}\", but it is not available");
                return; }

            //Use specific spawner if not null
            if (specificSpawner) {
                SpawnManager.SpawnUnit(unitPrefab, specificSpawner.transform.position, specificSpawner.transform.rotation, rallyPos); 
                return; }

            if (AvailableSpawners.Count == 0) {
                Debug.Log($"Tried to spawn unit, but there is no available spawner");
                return; }

            //isMatchUnitToSpawner = true : Si un spawner contient le nom de l'unité, on spawn dessus 
            if (isMatchUnitToSpawner) {
                foreach(GameObject spawner in AvailableSpawners) {
                    if (spawner.name.Contains(unitPrefab.name)) {
                        SpawnManager.SpawnUnit(unitPrefab, spawner.transform.position, spawner.transform.rotation, rallyPos); 
                        return; } } }

            //Default spawn
            SpawnManager.SpawnUnit(unitPrefab, AvailableSpawners[0].transform.position, AvailableSpawners[0].transform.rotation, rallyPos); 
        }

        public void SpawnUnitByName(string unitName, GameObject specificSpawner = null)
        { 
            foreach (var unitPrefab in AvailableUnits) {
                if (unitPrefab.name == unitName) {
                    SpawnUnit(unitPrefab, specificSpawner);
                    return; } }
            Debug.Log($"Tried to spawn unit \"{unitName}\", but it is not available");
        }

        public void SpawnUnitByIndex(int unitIndex, GameObject specificSpawner = null)
        {
            if (unitIndex < 0) {
                Debug.Log($"Tried to spawn index \"{unitIndex}\" unit, but index cannot be negative"); 
                return; }

            if (unitIndex > AvailableUnits.Count-1) {
                Debug.Log($"Tried to spawn index \"{unitIndex}\" unit, but the max index is {AvailableUnits.Count-1}"); 
                return; }

            SpawnUnit(AvailableUnits[unitIndex], specificSpawner);
        }
    }
}
