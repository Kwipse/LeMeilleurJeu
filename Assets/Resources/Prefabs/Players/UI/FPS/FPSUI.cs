using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FPSUI : UI
{
    FPSPlayer player;
    WeaponSystem WS;
    HealthSystem HS;

    AmmoSystem weaponAmmo;
    AmmoSystem backpackAmmo;

    public override void OnSetUI(GameObject FPSPlayer) 
    {
        player = FPSPlayer.GetComponent<FPSPlayer>();
        WS = FPSPlayer.GetComponent<WeaponSystem>();
        HS = FPSPlayer.GetComponent<HealthSystem>();

        //Subscribe to weapon system events
        WS.NewWeaponEquippedEvent += OnWeaponEquipped;
        WS.WeaponDestructionEvent += OnWeaponUnequipped;

        //Subscribe to health events
        HS.pv.OnValueChanged += OnHealthChanged;
        HS.pvMax.OnValueChanged += OnHealthChanged;

        if (WS.GetCurrentWeapon()) { OnWeaponEquipped(WS.GetCurrentWeapon()); }
        if (HS) { OnHealthChanged(0,0); }

        //Debug.Log($"{FPSPlayer.name} {WS}");
    }



    void UpdateAllTexts()
    {
        SetUIText("Health", $"{HS.pv.Value} / {HS.pvMax.Value}");
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
    void OnHealthChanged(int previous, int current) { SetUIText("Health", $"{HS.pv.Value} / {HS.pvMax.Value}"); }
}
