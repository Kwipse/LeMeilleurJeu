using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using scriptablesobjects;

[CreateAssetMenu]
public class FPSUI : UI
{
    //FPS Player info
    WeaponManager WM;

    AmmoSystem weaponAmmo;
    AmmoSystem backpackAmmo;


    public override void OnSetUI(GameObject FPSPlayer) 
    {
        WM = FPSPlayer.GetComponent<WeaponManager>();

        //Subscribe to weapon system events
        WM.NewWeaponEquippedEvent += OnWeaponEquipped;
        WM.WeaponDestructionEvent += OnWeaponUnequipped;
    }

    void UpdateAllTexts()
    {
        SetUIText("AmmoType", $"{WM.GetCurrentAmmoType()}");  
        SetUIText("WeaponAmmo", $"{weaponAmmo.GetAmmo()}");  
        SetUIText("BackpackAmmo", $" / {backpackAmmo.GetAmmo()}");  
    }


    void OnWeaponEquipped(GameObject newWeapon)
    {
        weaponAmmo = WM.GetCurrentWeaponAmmo();
        backpackAmmo = WM.GetCurrentBackpackAmmo();

        //Subscribe to ammo events
        weaponAmmo.GetAmmoRessource().ChangeEvent += OnNewWeaponAmmo;
        backpackAmmo.GetAmmoRessource().ChangeEvent += OnNewBackpackAmmo;

        //UI changes
        UpdateAllTexts();
        //Debug.Log($"FPSUI : Player equipped {newWeapon.name} , {weaponAmmo.GetAmmo()}/{backpackAmmo.GetAmmo()} {WM.GetCurrentAmmoType()} ammo");
    }

    void OnWeaponUnequipped(GameObject weapon)
    {
        //Unsubscribe to ammo events
        //Debug.Log($"FPSUI : Player unequipped {weapon.name}");
        if (!weapon) { return; }
        weaponAmmo.GetAmmoRessource().ChangeEvent -= OnNewWeaponAmmo;
        backpackAmmo.GetAmmoRessource().ChangeEvent -= OnNewBackpackAmmo;
    }


    void OnNewWeaponAmmo(int newWeaponAmmo) { SetUIText("WeaponAmmo", $"{newWeaponAmmo}");  }
    void OnNewBackpackAmmo(int newBackpackAmmo) { SetUIText("BackpackAmmo", $" / {newBackpackAmmo}"); }

}
