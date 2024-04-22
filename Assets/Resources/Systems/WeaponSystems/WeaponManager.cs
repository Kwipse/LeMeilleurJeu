using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;
using UnityEngine;
using scriptablesobjects;
using classes;

    public class WeaponManager : SyncedBehaviour, IWaitForGameSync
    {
        public WeaponSystem WS;

        [HideInInspector] public NetworkVariable<int> currentWeaponId = new NetworkVariable<int>(-1);

        GameObject currentWeapon;
        GameObject weaponHolder;
        int currentWeaponNumber;
        Arme currentWeaponScript;
        AmmoSystem currentWeaponAmmo;
        AmmoSystem currentBackpackAmmo;
        string currentAmmoType;

        float lastSwitchTime = -1f;

        Dictionary<Transform, Transform> currentHandles = new Dictionary<Transform, Transform>();
        Dictionary<Transform, IRigConstraint> holderHandles;
        List<Transform> holderStands, weaponStands, weaponHandles;

        //Events 
        public delegate void WeaponEvent(GameObject weapon);
        public event WeaponEvent NewWeaponEquippedEvent;
        public event WeaponEvent WeaponDestructionEvent;


        void Awake()
        {
            InitializeHolder();
        }

        void InitializeHolder()
        {
            weaponHolder = gameObject;
            //Debug.Log($"{WS.name} set to {weaponHolder.name} of client {weaponHolder.GetComponent<NetworkObject>().OwnerClientId}");

            //Stands & handles
            holderStands = new List<Transform>();
            holderHandles = new Dictionary<Transform, IRigConstraint>();
            foreach (var t in weaponHolder.GetComponentsInChildren<Transform>()) {
                if (t.tag == "WeaponStand") { holderStands.Add(t); } 
                if (t.tag == "WeaponHandle") { holderHandles.Add(t, t.parent.gameObject.GetComponent<IRigConstraint>()); } }

            //Check
            if (holderStands.Count == 0) Debug.Log($"{weaponHolder.name} has no stand");
            if (holderHandles.Count == 0) Debug.Log($"{weaponHolder.name} has no handle");

            //Init backpack
            if (WS.BackpackAmmos.Count == 0) { return; }
            foreach (var bp in WS.BackpackAmmos) { bp.SetAmmoToFull(); }
        }



        public override void StartAfterGameSync()
        {
            currentWeaponId.OnValueChanged += OnNewWeaponId;
            if (IsServer) { currentWeaponId.Value = -1; }
            if (IsOwner) { EquipWeaponNumber(0); }
        }

        public override void OnNetworkDespawn() 
        {
            currentWeaponId.OnValueChanged -= OnNewWeaponId; 
            UnequipWeapon();
        }


        public void OnNewWeaponId(int previous, int current)
        {
            currentWeapon = ObjectManager.GetObjectById(current);
            Debug.Log($"WeaponManager : New weapon {currentWeapon.name} : previousId = {previous}, currentId = {current}");
            InitWeapon();
            PlaceWeapon();
            SetIKs();
            if (NewWeaponEquippedEvent != null) { NewWeaponEquippedEvent(currentWeapon); }
        }

        void InitWeapon()
        {
            currentWeaponScript = currentWeapon.GetComponent<Arme>(); 
            currentWeaponAmmo = currentWeaponScript.GetMagasine();
            currentAmmoType = currentWeaponAmmo.GetAmmoType();
            currentBackpackAmmo = WS.GetBackpackAmmo(currentAmmoType);
            currentWeaponScript.SetWeaponManager(this);
            weaponStands = currentWeaponScript.GetWeaponStands();
            weaponHandles = currentWeaponScript.GetWeaponHandles();
        }

        //Weapon Placement
        void PlaceWeapon()
        {
            if (!IsOwner) { return; } //Owner Only
            foreach (var hs in holderStands)
                foreach (var ws in weaponStands)
                if (hs.gameObject.name == ws.gameObject.name) {
                    PlaceWeaponOnStand(hs, ws);
                    return; }
            //Debug.Log($"Holder has {holderStands.Count} stands, weapon has {weaponStands.Count} stands");
            PlaceWeaponOnStand(holderStands[0], weaponStands[0]); //Default to firsts stands
        }

        void PlaceWeaponOnStand(Transform holderStand, Transform weaponStand)
        {
            //Debug.Log($"WeaponManager : Parenting {currentWeapon} to {weaponHolder}"); 

            //Parenting constraint -> on ne parente pas avec la methode classique, sinon le réseau merde.
            ParentConstraint pc = currentWeapon.AddComponent<ParentConstraint>();
            pc.weight = 1;
            pc.locked = true;
            pc.constraintActive = true;

            ConstraintSource cs = new ConstraintSource();
            cs.sourceTransform = holderStand;
            cs.weight = 1;
            pc.AddSource(cs);

            currentWeapon.transform.position = holderStand.position;
            currentWeapon.transform.rotation = holderStand.rotation;
        }


        void SetIKs() {
            currentHandles = new Dictionary<Transform, Transform>();
            foreach (var hh in holderHandles) {
                hh.Value.weight = 0;
                foreach(var wh in weaponHandles) {
                    if (hh.Key.gameObject.name == wh.gameObject.name) {
                        hh.Value.weight = 1;
                        currentHandles.Add(hh.Key,wh);
                        //Debug.Log($"{weaponHolder.name} has setup {wh.name} for {currentWeapon.name}"); 
                    }
                }
            }
        }



        //Weapon Management
        public void EquipNextWeapon() { EquipWeaponNumber(((currentWeaponNumber + 1) + WS.AvailableWeapons.Count) % WS.AvailableWeapons.Count); }
        public void EquipPreviousWeapon() { EquipWeaponNumber(((currentWeaponNumber - 1) + WS.AvailableWeapons.Count) % WS.AvailableWeapons.Count); }
        public void EquipWeaponNumber(int weaponNumber) { EquipWeapon(WS.AvailableWeapons[weaponNumber]); }
        public void EquipWeapon(GameObject weaponPrefab) {
            if (Time.time - lastSwitchTime < 0.1f) { return; }
            lastSwitchTime = Time.time;
            UnequipWeapon();
            currentWeaponNumber = WS.AvailableWeapons.IndexOf(weaponPrefab);
            SpawnManager.SpawnWeapon(weaponPrefab, weaponHolder); }

        public void UnequipWeapon() {
            if (!IsOwner) {return;}
            if (!currentWeapon) {return;}
            if (WeaponDestructionEvent != null) { WeaponDestructionEvent(currentWeapon); }
            SpawnManager.DestroyObject(currentWeapon); } 


        //Manage Current Ammo
        public void AddAmmoInBackpack(int amount) { currentBackpackAmmo.AddAmmo(amount); }
        public void SetBackpackMaxAmmo(int maxAmount) { currentBackpackAmmo.SetMaxAmmo(maxAmount); }
        public void RemoveAmmoFromBackpack(int amount) { currentBackpackAmmo.RemoveAmmo(amount); }
        public bool IsEnoughBackpackAmmo(int testAmount) {return currentBackpackAmmo.IsEnoughAmmo(testAmount); }

        //Weapon Usage
        public void ShootWeapon() { currentWeaponScript.Shoot(false); }
        public void ShootAltWeapon() { currentWeaponScript.Shoot(true); }
        public void ReloadWeapon() { currentWeaponScript.StartReload(); }

        //Current Weapon Infos
        public GameObject GetWeaponHolder() { return weaponHolder; }
        public GameObject GetCurrentWeapon() { return currentWeapon; }
        public Arme GetCurrentWeaponScript() { return currentWeaponScript; }
        public AmmoSystem GetCurrentWeaponAmmo() { return currentWeaponAmmo; }
        public AmmoSystem GetCurrentBackpackAmmo() { return currentBackpackAmmo; }
        public string GetCurrentAmmoType() { return currentAmmoType; }
        public int GetCurrentWeaponNumber() { return currentWeaponNumber; }

        public void UpdateHandlesIK() //Call this in a fixedUpdate to Update weapon IKs positions
        {
            if (currentHandles.Count == 0) { return; }
            if (!currentWeapon) { return; }
            foreach (var ch in currentHandles) {
                ch.Key.position = ch.Value.position;
                ch.Key.rotation = ch.Value.rotation; } 
        }
    }
