using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using classes;
using systems;
using scriptablesobjects;

[CreateAssetMenu]
public class FPSUI : UI
{
    //FPS Player info
    WeaponSystem WS;
    GameObject currentWeapon;

    AmmoSystem weaponAmmo;
    AmmoSystem backpackAmmo;


    public override void OnSetUI(GameObject FPSPlayer) 
    {
        WS = FPSPlayer.GetComponent<WeaponSystem>();

        //Subscribe to weapon system events
        WS.NewWeaponEquippedEvent += OnWeaponEquipped;
        WS.WeaponDestructionEvent += OnWeaponUnequipped;
    }

    void UpdateAllTexts()
    {
        SetUIText("AmmoType", $"{WS.GetCurrentAmmoType()}");  
        SetUIText("WeaponAmmo", $"{weaponAmmo.GetAmmo()}");  
        SetUIText("BackpackAmmo", $" / {backpackAmmo.GetAmmo()}");  
    }


    void OnWeaponEquipped(GameObject newWeapon)
    {
        weaponAmmo = WS.GetCurrentWeaponAmmo();
        backpackAmmo = WS.GetCurrentBackpackAmmo();

        //Subscribe to ammo events
        weaponAmmo.GetAmmoRessource().ChangeEvent += OnNewWeaponAmmo;
        backpackAmmo.GetAmmoRessource().ChangeEvent += OnNewBackpackAmmo;

        //UI changes
        UpdateAllTexts();
        //Debug.Log($"UI : {newWeapon.name} equipped, {weaponAmmo.GetAmmo()}/{backpackAmmo.GetAmmo()} {WS.GetCurrentAmmoType()} ammo");
    }

    void OnWeaponUnequipped(GameObject weapon)
    {
        //Unsubscribe to ammo events
        weaponAmmo.GetAmmoRessource().ChangeEvent -= OnNewWeaponAmmo;
        backpackAmmo.GetAmmoRessource().ChangeEvent -= OnNewBackpackAmmo;
    }


    void OnNewWeaponAmmo(int newWeaponAmmo) { SetUIText("WeaponAmmo", $"{newWeaponAmmo}");  }
    void OnNewBackpackAmmo(int newBackpackAmmo) { SetUIText("BackpackAmmo", $" / {newBackpackAmmo}"); }

}
