using System.Collections.Generic;
using UnityEngine;

namespace scriptablesobjects {

    [CreateAssetMenu]
    public class WeaponSystem : ScriptableObject
    {
        //Définir les armes par defaut dans l'éditeur
        public List<GameObject> AvailableWeapons;
        public List<AmmoSystem> BackpackAmmos;
        
        //Availables Weapons
        public void AddAvailableWeapon(GameObject weaponPrefab) { AvailableWeapons.Add(weaponPrefab); }
        public void RemoveAvailableWeapon(GameObject weaponPrefab) { AvailableWeapons.Remove(weaponPrefab); }

        //Availables Ammos
        public void AddAvailableAmmo(string ammoType, int maxAmmo) { if (!IsAmmoAvailable(ammoType)) { BackpackAmmos.Add(new AmmoSystem(ammoType, maxAmmo)); } }
        public void RemoveAvailableAmmo(string ammoType) { if (IsAmmoAvailable(ammoType)) { BackpackAmmos.Remove(GetBackpackAmmo(ammoType)); } }

        public bool IsAmmoAvailable(string ammoType) { return GetBackpackAmmo(ammoType) ? true : false; }
        public AmmoSystem GetBackpackAmmo(string ammoType) { foreach (var ammo in BackpackAmmos) { if (ammo.ammoType == ammoType) { return ammo; } } return null; }
    }
} 
