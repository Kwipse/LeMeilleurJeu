using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Unity.Netcode;
using classes;
using scriptablesobjects;

namespace systems {

    public class WeaponSystem : NetworkBehaviour
    {
        //Définir les armes par defaut dans l'éditeur
        public List<GameObject> AvailableWeapons;
        public List<AmmoSystem> BackpackAmmos;
        

        GameObject currentWeapon;
        int currentWeaponNumber;
        Arme currentWeaponScript;
        AmmoSystem currentWeaponAmmo;
        AmmoSystem currentBackpackAmmo;
        string currentAmmoType;

        Animator anim;

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
            holderStands = new List<Transform>();
            holderHandles = new Dictionary<Transform, IRigConstraint>();
            foreach (var t in GetComponentsInChildren<Transform>()) {
                if (t.tag == "WeaponStand") { holderStands.Add(t); } 
                if (t.tag == "WeaponHandle") { holderHandles.Add(t, t.parent.gameObject.GetComponent<IRigConstraint>()); } }

            //Check for holder handles
            if (holderStands.Count == 0) Debug.Log($"{gameObject.name} has no stand");
            if (holderHandles.Count == 0) Debug.Log($"{gameObject.name} has no handle");
        }

        void Start()
        {
            foreach (var bp in BackpackAmmos) { bp.SetAmmoToFull(); }
            EquipWeaponNumber(0);
        }

        public override void OnDestroy()
        {
            if (IsServer) SpawnManager.DestroyObject(currentWeapon);
            base.OnDestroy();
        }

        void Update()
        {
            UpdateHandlesIK(); 
        }




        //Callback from spawn manager
        public void SetupEquippedWeapon(GameObject weapon) 
        {
            currentWeapon = weapon;
            SetWeaponRpc(currentWeapon, RpcTarget.Everyone);
            PlaceWeapon();
            //Debug.Log($"{gameObject.name} has changed weapon to {weapon.name}");

            //send NewWeapon event
            if (NewWeaponEquippedEvent != null) { NewWeaponEquippedEvent(currentWeapon); }
        }



        //Synchronize current weapon on new clients
        public override void OnNetworkSpawn() {
            if (IsServer) { NetworkManager.OnClientConnectedCallback += OnNewClientConnected; } }

        public override void OnNetworkDespawn() {
            if (IsServer) { NetworkManager.OnClientConnectedCallback -= OnNewClientConnected; } }

        void OnNewClientConnected(ulong clientId)
        {
            //Debug.Log($"WeaponSystem : client {clientId} connected ");
            SetWeaponRpc(currentWeapon, RpcTarget.Single(clientId, RpcTargetUse.Temp));
        }



        //Synchronize RPC
        [Rpc(SendTo.SpecifiedInParams)]
        void SetWeaponRpc(NetworkObjectReference weaponNor, RpcParams rpcParams)
        {
            currentWeapon = weaponNor;
            InitializeWeapon();
            SetIKs();
        }



        void InitializeWeapon()
        {
            currentWeaponScript = currentWeapon.GetComponent<Arme>(); 
            currentWeaponAmmo = currentWeaponScript.GetMagasine();
            currentAmmoType = currentWeaponAmmo.GetAmmoType();
            currentBackpackAmmo = GetBackpackAmmo(currentAmmoType);
            currentWeaponScript.SetWeaponSystem(this);

            weaponStands = currentWeaponScript.GetWeaponStands();
            weaponHandles = currentWeaponScript.GetWeaponHandles();
        }

        void SetIKs()
        {
            currentHandles = new Dictionary<Transform, Transform>();
            foreach (var hh in holderHandles) {
                hh.Value.weight = 0;
                foreach(var wh in weaponHandles) {
                    if (hh.Key.gameObject.name == wh.gameObject.name) {
                        hh.Value.weight = 1;
                        currentHandles.Add(hh.Key,wh);
                        //Debug.Log($"{gameObject.name} has setup {wh.name} for {currentWeapon.name}"); 
                    }
                }
            }
        }

        void UpdateHandlesIK() 
        {
            if (currentHandles.Count == 0) { return; }
            if (!currentWeapon) { return; }
            foreach (var ch in currentHandles) {
                ch.Key.position = ch.Value.position;
                ch.Key.rotation = ch.Value.rotation; } 
        }



        //Weapon Placement
        void PlaceWeapon()
        {
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
            currentWeapon.transform.parent = holderStand;
            currentWeapon.transform.position = holderStand.position;
            currentWeapon.transform.rotation = holderStand.rotation;
        }



        //Availables Weapons
        public void AddAvailableWeapon(GameObject weaponPrefab) { AvailableWeapons.Add(weaponPrefab); }
        public void RemoveAvailableWeapon(GameObject weaponPrefab) { AvailableWeapons.Remove(weaponPrefab); }

        //Availables Ammos
        public void AddAvailableAmmo(string ammoType, int maxAmmo) { if (!IsAmmoAvailable(ammoType)) { BackpackAmmos.Add(new AmmoSystem(ammoType, maxAmmo)); } }
        public void RemoveAvailableAmmo(string ammoType) { if (IsAmmoAvailable(ammoType)) { BackpackAmmos.Remove(GetBackpackAmmo(ammoType)); } }

        //Weapon Management
        public void EquipWeaponNumber(int weaponNumber) { EquipWeapon(AvailableWeapons[weaponNumber]); }
        public void EquipNextWeapon() { EquipWeaponNumber(((currentWeaponNumber + 1) + AvailableWeapons.Count) % AvailableWeapons.Count); }
        public void EquipPreviousWeapon() { EquipWeaponNumber(((currentWeaponNumber - 1) + AvailableWeapons.Count) % AvailableWeapons.Count); }
        public void EquipWeapon(GameObject weaponPrefab) {
            currentWeaponNumber = AvailableWeapons.IndexOf(weaponPrefab);
            if (!IsOwner) { return; } 
            if (currentWeapon) {
                //send NewWeapon event
                if (WeaponDestructionEvent != null) { WeaponDestructionEvent(currentWeapon); }
                SpawnManager.DestroyObject(currentWeapon); 
            }
            SpawnManager.SpawnWeapon(weaponPrefab, this.gameObject); }

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
        public GameObject GetCurrentWeapon() { return currentWeapon; }
        public Arme GetCurrentWeaponScript() { return currentWeaponScript; }
        public AmmoSystem GetCurrentWeaponAmmo() { return currentWeaponAmmo; }
        public AmmoSystem GetCurrentBackpackAmmo() { return currentBackpackAmmo; }
        public string GetCurrentAmmoType() { return currentAmmoType; }
        public int GetCurrentWeaponNumber() { return currentWeaponNumber; }


        bool IsAmmoAvailable(string ammoType) { return GetBackpackAmmo(ammoType) ? true : false; }
        AmmoSystem GetBackpackAmmo(string ammoType) { foreach (var ammo in BackpackAmmos) { if (ammo.ammoType == ammoType) { return ammo; } } return null; }




    


    }
} 
