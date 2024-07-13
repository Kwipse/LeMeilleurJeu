using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using scriptablesobjects;

public class WeaponSystem : SyncedBehaviour, IWaitForGameSync
{
    public WeaponSystemAsset WSA;

    List<GameObject> AvailableWeapons;
    List<AmmoSystem> BackpackAmmos;
    [HideInInspector] public NetworkVariable<int> currentWeaponId;

    GameObject holder;

    void Awake()
    {
        holder = gameObject;
        AvailableWeapons = WSA.AvailableWeapons;
        BackpackAmmos = WSA.BackpackAmmos;
        currentWeaponId = new NetworkVariable<int>();
        InitBackpack();
        InitHolderStands();
        InitHolderHandles();
    }

    public override void StartAfterGameSync()
    {
        InitWeapon();

    }

    void InitWeapon()
    {
        if (IsServer) { currentWeaponId.Value = -1; }
        currentWeaponId.OnValueChanged += OnWeaponChanged;
        if (IsOwner) { EquipWeaponNumber(0); }
        if (!IsServer && !IsOwner) { OnWeaponChanged(-2, currentWeaponId.Value); }
    }

    void OnWeaponChanged(int previous, int current) //Called on currentWeaponId change
    {
        //Debug.Log($"WeaponId changed from {previous} to {current}");
        UpdateWeaponInfo(current);
        UpdateWeaponStands();
        UpdateWeaponHandles();
        if (NewWeaponEquippedEvent != null) { NewWeaponEquippedEvent(currentWeapon); }
    }

    public override void OnDestroy()
    {
        UnequipWeapon();
        currentWeaponId.OnValueChanged -= OnWeaponChanged;
        base.OnDestroy();
    }



    //BackPack
    public void AddAvailableWeapon(GameObject weaponPrefab) { AvailableWeapons.Add(weaponPrefab); }
    public void RemoveAvailableWeapon(GameObject weaponPrefab) { AvailableWeapons.Remove(weaponPrefab); }
    public void AddAvailableAmmo(string ammoType, int maxAmmo) { if (!IsAmmoAvailable(ammoType)) { BackpackAmmos.Add(new AmmoSystem(ammoType, maxAmmo)); } }
    public void RemoveAvailableAmmo(string ammoType) { if (IsAmmoAvailable(ammoType)) { BackpackAmmos.Remove(GetBackpackAmmo(ammoType)); } }
    public bool IsAmmoAvailable(string ammoType) { return GetBackpackAmmo(ammoType) ? true : false; }
    public AmmoSystem GetBackpackAmmo(string ammoType) { foreach (var ammo in BackpackAmmos) { if (ammo.ammoType == ammoType) { return ammo; } } return null; }

    void InitBackpack()
    {
        if (BackpackAmmos.Count == 0) { return; }
        foreach (var bp in BackpackAmmos) { bp.SetAmmoToFull(); }
    }

    public void AddAmmoInBackpack(int amount) { currentBackpackAmmo.AddAmmo(amount); }
    public void SetBackpackMaxAmmo(int maxAmount) { currentBackpackAmmo.SetMaxAmmo(maxAmount); }
    public void RemoveAmmoFromBackpack(int amount) { currentBackpackAmmo.RemoveAmmo(amount); }
    public bool IsEnoughBackpackAmmo(int testAmount) {return currentBackpackAmmo.IsEnoughAmmo(testAmount); }



    //Stands
    List<Transform> holderStands;
    List<Transform> weaponStands;

    void InitHolderStands()
    {
        holderStands = new List<Transform>();

        foreach (var t in holder.GetComponentsInChildren<Transform>()) {
            if (t.tag == "WeaponStand") { holderStands.Add(t); } }

        if (holderStands.Count == 0) Debug.Log($"{holder.name} has no stand");
    }

    void UpdateWeaponStands()
    {
        weaponStands = new List<Transform>();
        weaponStands = currentWeaponScript.GetWeaponStands();
        PlaceWeapon();
    }

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
        //Debug.Log($"WeaponSystem : Parenting {currentWeapon} to {weaponHolder}"); 

        //Parenting constraint -> on ne parente pas avec la methode classique, sinon le réseau merde lors du destroy/despawn
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
        currentWeapon.transform.localScale = holderStand.localScale; 
    }



    //Handles
    List<Transform> weaponHandles;
    Dictionary<Transform, IRigConstraint> holderHandles;
    Dictionary<Transform, Transform> currentHandles;

    void InitHolderHandles()
    {
        holderHandles = new Dictionary<Transform, IRigConstraint>();
        foreach (var t in holder.GetComponentsInChildren<Transform>()) {
            if (t.tag == "WeaponHandle") { holderHandles.Add(t, t.parent.gameObject.GetComponent<IRigConstraint>()); } }

        if (holderHandles.Count == 0) Debug.Log($"{holder.name} has no handle");
    }

    void UpdateWeaponHandles()
    {
        weaponHandles = new List<Transform>();
        weaponHandles = currentWeaponScript.GetWeaponHandles();
        SetIKs();
    }

    void SetIKs() //Populate currentHandles, activate relevant IK constraints
    {
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

    public void UpdateHandlesIK() //Call this in a fixedUpdate to Update weapon IKs positions
    {
        if (!currentWeapon) { return; }
        if (currentHandles.Count == 0) { return; }
        foreach (var ch in currentHandles) {
            ch.Key.position = ch.Value.position;
            ch.Key.rotation = ch.Value.rotation; } 
    }



    //Current weapon
    //int currentWeaponId;
    GameObject currentWeapon;
    int currentWeaponNumber;
    Arme currentWeaponScript;
    AmmoSystem currentWeaponAmmo;
    AmmoSystem currentBackpackAmmo;
    string currentAmmoType;
    float lastSwitchTime = -1f;

    public delegate void WeaponEvent(GameObject weapon);
    public event WeaponEvent NewWeaponEquippedEvent;
    public event WeaponEvent WeaponDestructionEvent;



    public GameObject GetCurrentWeapon() { return currentWeapon; }
    public int GetCurrentWeaponNumber() { return currentWeaponNumber; }
    public Arme GetCurrentWeaponScript() { return currentWeaponScript; }
    public AmmoSystem GetCurrentWeaponAmmo() { return currentWeaponAmmo; }
    public AmmoSystem GetCurrentBackpackAmmo() { return currentBackpackAmmo; }
    public string GetCurrentAmmoType() { return currentAmmoType; }

    public void EquipNextWeapon() { EquipWeaponNumber(((currentWeaponNumber + 1) + AvailableWeapons.Count) % AvailableWeapons.Count); }
    public void EquipPreviousWeapon() { EquipWeaponNumber(((currentWeaponNumber - 1) + AvailableWeapons.Count) % AvailableWeapons.Count); }
    public void EquipWeaponNumber(int weaponNumber) { EquipWeapon(AvailableWeapons[weaponNumber]); }
    public void EquipWeapon(GameObject weaponPrefab) {
        if (!IsOwner) { return; }
        if (Time.time - lastSwitchTime < 0.1f) { return; }
        lastSwitchTime = Time.time;
        UnequipWeapon();
        currentWeaponNumber = AvailableWeapons.IndexOf(weaponPrefab);
        SpawnManager.SpawnWeapon(weaponPrefab, holder); }

    public void UnequipWeapon() {
        if (!IsOwner) {return;}
        if (!currentWeapon) {return;}
        if (WeaponDestructionEvent != null) { WeaponDestructionEvent(currentWeapon); }
        SpawnManager.DestroyObject(currentWeapon); } 


    void UpdateWeaponInfo(int weaponId)
    {
        currentWeapon = ObjectManager.GetObjectById(weaponId);
        currentWeaponScript = currentWeapon.GetComponent<Arme>(); 
        currentWeaponAmmo = currentWeaponScript.GetMagasine();
        currentAmmoType = currentWeaponAmmo.GetAmmoType();
        currentBackpackAmmo = GetBackpackAmmo(currentAmmoType);
        currentWeaponScript.SetWeaponSystem(this);
        weaponStands = currentWeaponScript.GetWeaponStands();
        weaponHandles = currentWeaponScript.GetWeaponHandles();
    }


    //Weapon Usage
    public void ShootWeapon() { currentWeaponScript.Shoot(false); }
    public void ShootAltWeapon() { currentWeaponScript.Shoot(true); }
    public void ReloadWeapon() { currentWeaponScript.StartReload(); }


}
