using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using scriptablesobjects;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]

public abstract class Arme : SyncedBehaviour, IWaitForGameSync 
{
    //Ammo properties
    public AmmoSystem magasineAmmo;
    public float reloadTime;
    public int reloadAmount = 0; //0 -> fullMag

    //Shooting properties
    public float CadenceDeTir;
    public float CadenceDeTirAlt;
    public int ShootAmmo;
    public int ShootAltAmmo;

    //Reload Options
    public bool autoReloadOnMagasineEmpty = true;
    public bool autoReloadOnMagasineNotFull = false;
    public bool keepMagAmmoOnReload = false;

    WeaponSystem WS;
    AmmoSystem backpackAmmo;
    Transform weaponGunpoint;
    float lastShoot;

    List<Transform> weaponStands, weaponHandles;


    public virtual void Awake()
    {
        //Debug.Log($"Weapon is awake");
        InitWeaponInfos();
    }

    public override void StartAfterGameSync()
    {

        ColorManager.SetObjectColors(gameObject);
        if (!IsOwner) { enabled = false; return; } 

        lastShoot = Time.time - Mathf.Max(CadenceDeTir, CadenceDeTirAlt); 

        //Subscribe to magasineAmmo events
        magasineAmmo.GetAmmoRessource().ChangeEvent += OnMagAmmoChanged;
        magasineAmmo.GetAmmoRessource().HitMinEvent += OnMagAmmoEmpty;
        OnMagAmmoChanged(magasineAmmo.GetAmmo()); 
    }

    void InitWeaponInfos()
    {
        weaponStands = new List<Transform>();
        weaponHandles = new List<Transform>();
        weaponGunpoint = null;

        foreach (var t in gameObject.GetComponentsInChildren<Transform>()) {
            if (t.tag == "WeaponHandle") { weaponHandles.Add(t); } 
            if (t.tag == "WeaponStand") { weaponStands.Add(t); } 
            if (t.tag == "Gunpoint") { weaponGunpoint = t;  } }

        if (weaponStands.Count == 0) { Debug.Log("Weapon has no stand"); }
        if (weaponHandles.Count == 0) { Debug.Log("Weapon has no handle"); }
        if (!weaponGunpoint) { Debug.Log("Weapon has no handle"); }
    }


    public override void OnDestroy()
    {
        magasineAmmo.GetAmmoRessource().ChangeEvent -= OnMagAmmoChanged;
        magasineAmmo.GetAmmoRessource().HitMinEvent -= OnMagAmmoEmpty;

        StopReload();
        base.OnDestroy();
    }



    //Infos
    public AmmoSystem GetMagasine() { return magasineAmmo; }
    public int GetAmmo() {return magasineAmmo.GetAmmo(); }
    public Transform GetGunpointTransform() { return weaponGunpoint; }
    public WeaponSystem GetWeaponSystem() { return WS; }
    public List<Transform> GetWeaponHandles() {return weaponHandles; }
    public List<Transform> GetWeaponStands() {return weaponStands; }


    //Fonctions a implementer dans vos scripts d'arme
    public abstract void OnShoot(); //obligatoire
    public virtual void OnShootAlt() {} //optionnel



    //Called from WeaponSystem
    public void SetWeaponSystem(WeaponSystem ws) 
    {
        WS = ws;
        backpackAmmo = ws.GetBackpackAmmo(magasineAmmo.GetAmmoType());
        //Debug.Log($"{gameObject.name} is set to {ws.GetWeaponHolder().name}");
    }




    //Events
    void OnMagAmmoChanged(int newMagAmmoAmount)
    {
        //Debug.Log($"{gameObject.name } has {newMagAmmoAmount} munitions in magasine");
        if (autoReloadOnMagasineNotFull && !magasineAmmo.IsAmmoFull()) {
            StartReload(); }
    }

    void OnMagAmmoEmpty()
    {
        //Debug.Log($"{gameObject.name } has {newMagAmmoAmount} munitions in magasine");
        if ((autoReloadOnMagasineEmpty) && (!autoReloadOnMagasineNotFull)) {
            StartReload(); }
    }



    //Shoot
    public void Shoot(bool AltShoot = false)
    {
        int ammo = !AltShoot ? ShootAmmo : ShootAltAmmo;
        float cadence = !AltShoot ? CadenceDeTir : CadenceDeTirAlt;

        //Shoot conditions
        if (!magasineAmmo.IsEnoughAmmo(ammo)) { return; }
        if ((Time.time - lastShoot) < cadence) { return; }

        StopReload();
        lastShoot = Time.time;

        if (!AltShoot) OnShoot();
        if (AltShoot) OnShootAlt();

        magasineAmmo.RemoveAmmo(ammo);
        return;
    }

    public void ShootTargetPoint(Vector3 targetPoint)
    {
        gameObject.transform.LookAt(targetPoint);
        Shoot();
    }

    public void ShootTargetObject(GameObject targetGo)
    {
        gameObject.transform.LookAt(targetGo.GetComponent<Collider>().bounds.center);
        Shoot();
    }


    //Reload
    public void StartReload() { Invoke("Reload", reloadTime); }
    public void StopReload() { CancelInvoke("Reload"); }

    void Reload() 
    {
        int tmpAmmo;
        int bpAmmo = backpackAmmo.GetAmmo();
        int wAmmo = magasineAmmo.GetAmmo();
        int maxAmmo = magasineAmmo.GetMaxAmmo();
        int rAmmo = (reloadAmount == 0) ? maxAmmo : reloadAmount ;
        int wastedAmmo = Mathf.Max(0,wAmmo + rAmmo - maxAmmo);

        //If backpack empty, abort reload
        if (bpAmmo == 0) { return; }

        //If not enough for full reload
        if (bpAmmo < rAmmo) { tmpAmmo = bpAmmo; }
        else { tmpAmmo = rAmmo; }

        //Manage ammos
        if (!keepMagAmmoOnReload) { backpackAmmo.RemoveAmmo(tmpAmmo); }
        if (keepMagAmmoOnReload) { backpackAmmo.RemoveAmmo(tmpAmmo - wastedAmmo); }
        magasineAmmo.AddAmmo(tmpAmmo); }
}
