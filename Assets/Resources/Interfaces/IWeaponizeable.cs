using UnityEngine;

namespace interfaces {

    //Je laisse ça là pour l'exemple, mais cette interface ne sert plus a rien

    public interface IWeaponizeable
    {
        GameObject currentWeapon {get; set; }

        void EquipWeapon(GameObject weapon)
        {
            currentWeapon = weapon;
        }
    }
}
