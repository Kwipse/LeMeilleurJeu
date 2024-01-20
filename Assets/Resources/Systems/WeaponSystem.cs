using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Unity.Netcode;
using classes;

namespace systems {

    public class WeaponSystem : NetworkBehaviour
    {
        //Définir les armes par defaut dans l'éditeur
        public List<GameObject> AvailableWeapons;

        [HideInInspector]
        public GameObject currentWeapon;

        int currentWeaponNumber;
        Arme currentWeaponScript;
        Animator anim;

        //Handles
        Dictionary<Transform, IRigConstraint> holderHandles;
        List<Transform> weaponHandles;
        Dictionary<Transform, Transform> currentHandles;

        //Stands
        List<Transform> holderStands;
        List<Transform> weaponStands;


        void Awake()
        {
            InitializeHolderStands();
            InitializeHolderHandles();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                this.enabled = false;
        }

        void Start()
        {
            EquipWeaponNumber(0);
        }

        void Update()
        {
            UpdateHandlesIK();
        }


        //Availables Weapons
        public void AddAvailableWeapon(GameObject weaponPrefab) {
            AvailableWeapons.Add(weaponPrefab); }

        public void RemoveAvailableWeapon(GameObject weaponPrefab) {
            AvailableWeapons.Remove(weaponPrefab); }


        //Callback from spawn manager
        public void SetupEquippedWeapon(GameObject weapon) {
            currentWeapon = weapon;
            currentWeaponScript = weapon.GetComponent<Arme>(); 
            InitializeWeaponStands();
            InitializeWeaponHandles();
            PlaceWeapon();

            //Debug.Log($"{gameObject.name} has setup weapon {weapon.name}");
        }


        //Weapon Management
        public void EquipWeapon(GameObject weaponPrefab) {
            SpawnManager.DestroyObject(currentWeapon);
            SpawnManager.SpawnWeapon(weaponPrefab, this.gameObject); }

        public void EquipWeaponNumber(int weaponNumber) {
            EquipWeapon(AvailableWeapons[weaponNumber]);
            currentWeaponNumber = weaponNumber; }

        public void EquipNextWeapon() {
            EquipWeaponNumber(((currentWeaponNumber + 1) + AvailableWeapons.Count) % AvailableWeapons.Count); }

        public void EquipPreviousWeapon() {
            EquipWeaponNumber(((currentWeaponNumber - 1) + AvailableWeapons.Count) % AvailableWeapons.Count); }


        //Weapon Usage
        public void ShootWeapon() {
            currentWeaponScript.Shoot(false); }

        public void ShootAltWeapon() {
            currentWeaponScript.Shoot(true); }

        public void ReloadWeapon() {
            currentWeaponScript.StartReload(); }


        //Weapon Placement
        void InitializeHolderStands()
        {
            holderStands = new List<Transform>();
            foreach (Transform t in GetComponentsInChildren<Transform>())
                if (t.gameObject.tag == "WeaponStand")
                    holderStands.Add(t);

            if (holderStands.Count == 0)
                Debug.Log("Weapon Holder has no stand");
        }

        void InitializeWeaponStands() {
            weaponStands = new List<Transform>();
            foreach (Transform t in currentWeapon.transform.GetComponentsInChildren<Transform>()) {
                if (t.gameObject.tag == "WeaponStand")
                    weaponStands.Add(t); }

            if (weaponStands.Count == 0)
                Debug.Log("Weapon has no stand");
        }

        void PlaceWeapon() {
            foreach (Transform hs in holderStands)
                foreach (Transform ws in weaponStands)
                if (hs.gameObject.name == ws.gameObject.name) {
                    //Parent weapon to stand
                    currentWeapon.transform.parent = hs;
                    currentWeapon.transform.position = hs.position;
                    currentWeapon.transform.rotation = hs.rotation;
                }
        }


        //Weapon Animations
        void InitializeHolderHandles()
        {
            //Find holder handles
            holderHandles = new Dictionary<Transform, IRigConstraint>();
            foreach (Transform t in GetComponentsInChildren<Transform>()) {
                if (t.gameObject.tag == "WeaponHandle") {
                    holderHandles.Add(t, t.parent.gameObject.GetComponent<IRigConstraint>()); } }

            //Check for holder handles
            if (holderHandles.Count == 0)
                Debug.Log("Weapon Holder has no handle");
        }

        void InitializeWeaponHandles() 
        {
            //Find weapon handles
            weaponHandles = new List<Transform>();
            foreach (Transform t in currentWeapon.GetComponentsInChildren<Transform>()) {
                if (t.gameObject.tag == "WeaponHandle") {
                    weaponHandles.Add(t); } }

            //If weapon has no handle
            if (weaponHandles.Count == 0) {
                Debug.Log("Weapon has no handle");
                return;}


            //If weapon has handles
            currentHandles = new Dictionary<Transform, Transform>();
            foreach (var hh in holderHandles) {
                hh.Value.weight = 0;
                foreach(var wh in weaponHandles) {
                    if (hh.Key.gameObject.name == wh.gameObject.name) 
                    {
                        currentHandles.Add(hh.Key, wh);
                        hh.Value.weight = 1;
                        Debug.Log($"{gameObject.name} has setup {wh.name} for {currentWeapon.name}");
                    }
                }
            }
        }

        void UpdateHandlesIK()
        {
            foreach (var ch in currentHandles) {
                ch.Key.position = ch.Value.position;
                ch.Key.rotation = ch.Value.rotation;
            }
        }


    }
}
