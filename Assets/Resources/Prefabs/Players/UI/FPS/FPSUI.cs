using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using scriptablesobjects;

[CreateAssetMenu]
public class FPSUI : UI
{
    FPSPlayer player;
    WeaponSystem WS;

    AmmoSystem weaponAmmo;
    AmmoSystem backpackAmmo;


    public override void OnSetUI(GameObject FPSPlayer) 
    {
        player = FPSPlayer.GetComponent<FPSPlayer>();
        WS = FPSPlayer.GetComponent<IWeaponizable>().WS;
        //Debug.Log($"{FPSPlayer.name} {WS}");

        //Subscribe to weapon system events
        WS.NewWeaponEquippedEvent += OnWeaponEquipped;
        WS.WeaponDestructionEvent += OnWeaponUnequipped;

        if (WS.GetCurrentWeapon())
        {
            OnWeaponEquipped(WS.GetCurrentWeapon());
        }
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
        //Debug.Log($"FPSUI : Player equipped {newWeapon.name} , {weaponAmmo.GetAmmo()}/{backpackAmmo.GetAmmo()} {WS.GetCurrentAmmoType()} ammo");
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
