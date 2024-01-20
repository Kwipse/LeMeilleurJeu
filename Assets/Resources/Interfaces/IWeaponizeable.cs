using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace interfaces {

    public interface IWeaponizeable
    {
        GameObject currentWeapon {get; set; }

        void EquipWeapon(GameObject weapon)
        {
            currentWeapon = weapon;
        }
    }
}
