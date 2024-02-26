using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using systems;
using scriptablesobjects;

namespace classes {

    [RequireComponent(typeof(FPSCamera))]
    [RequireComponent(typeof(FPSMovement))]
    [RequireComponent(typeof(FPSAnimator))]
    [RequireComponent(typeof(WeaponSystem))]

    public class FPSPlayer : NetworkBehaviour
    {
        public FPSUI UI;
        WeaponSystem WS;

        void Awake()
        {
            WS = GetComponent<WeaponSystem>();
        }

        public override void OnNetworkSpawn()
        {
            ColorManager.SetObjectColors(gameObject);

            if (!IsOwner)
            {
                GetComponentInChildren<Camera>().enabled = false;
                enabled = false;
            }
        }

        void Start()
        {
            UI.SetUI(gameObject);
        }

        void Update()
        {
            PlayerInputs();
        }

        void PlayerInputs()
        {
            if (Input.GetMouseButton(0)) WS.ShootWeapon();
            if (Input.GetMouseButton(1)) WS.ShootAltWeapon();
            if (Input.GetKeyDown(KeyCode.R)) WS.ReloadWeapon(); 
            if (Input.GetKeyDown(KeyCode.T)) WS.GetCurrentBackpackAmmo().SetAmmoToFull(); 
            if (Input.mouseScrollDelta.y > 0) WS.EquipNextWeapon();
            if (Input.mouseScrollDelta.y < 0) WS.EquipPreviousWeapon();
        }
    }
}
