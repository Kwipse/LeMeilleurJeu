using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using scriptablesobjects;

namespace classes {

    [RequireComponent(typeof(FPSCamera))]
    [RequireComponent(typeof(FPSAnimator))]

    public class FPSPlayer : SyncedBehaviour, IWaitForGameSync
    {
        public FPSUI UI;
        public FPSMovement MV;
        [HideInInspector] public WeaponManager WM;


        void Awake()
        {
            enabled = false;
        }

        public override void StartAfterGameSync()
        {
            ColorManager.SetObjectColors(gameObject);

            if (!IsOwner) {
                GetComponentInChildren<Camera>().enabled = false;
                return; }

            if (IsOwner)
            {
                enabled = true;
            }

            UI.SetUI(gameObject);
            WM = GetComponent<WeaponManager>();
            MV.SetMovingObject(gameObject);
        }

        void Update()
        {
            PlayerInputs();
        }

        void FixedUpdate()
        {
            MV.UpdatePosition();
        }

        void PlayerInputs()
        {
            if (Input.GetMouseButton(0)) WM.ShootWeapon();
            if (Input.GetMouseButton(1)) WM.ShootAltWeapon();
            if (Input.GetKeyDown(KeyCode.R)) WM.ReloadWeapon(); 
            if (Input.GetKeyDown(KeyCode.T)) WM.GetCurrentBackpackAmmo().SetAmmoToFull(); 
            if (Input.mouseScrollDelta.y > 0) WM.EquipNextWeapon();
            if (Input.mouseScrollDelta.y < 0) WM.EquipPreviousWeapon();

            if (Input.GetKeyDown(KeyCode.Space)) MV.Jump();
            if (Input.GetKey(KeyCode.Z)) MV.MoveForward();
            if (Input.GetKey(KeyCode.Q)) MV.MoveLeft();
            if (Input.GetKey(KeyCode.S)) MV.MoveBackward();
            if (Input.GetKey(KeyCode.D)) MV.MoveRight();
        }


        void OnCollisionEnter(Collision col) {
            if (col.gameObject.tag == "ground") {
                MV.RechargeJumps(); } }
    }
}
