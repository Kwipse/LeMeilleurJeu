using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using interfaces;

namespace classes {

    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(ClientNetworkTransform))]
    
    public abstract class Arme : NetworkBehaviour
    {
        [HideInInspector]
        public GameObject weaponHolder;

        public float CadenceDeTir;
        public int ShootAmmo;
        public float CadenceDeTirAlt;
        public int ShootAltAmmo;

        public float DureeRechargement;
        public int QteRechargement;
        public int TailleChargeur;

        GameObject weapon;
        Transform weaponGunpoint;
        float lastShoot;
        int chargeur;

        //Pour placer l'arme
        List<Transform> weaponStands;
        Transform FPSShoulderStand, FPSSideStand, FPSFrontStand;
        Transform UnitStand;

        Transform targetStand;
        Animator anim;

        public virtual void Awake()
        {

        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) 
                enabled = false;
        }


        public virtual void Start()
        {
            anim = weaponHolder.GetComponent<Animator>();

            InitWeapon();
            SetWeaponToHolder();
        }


        void InitWeapon()
        {
            weapon = gameObject;
            chargeur = TailleChargeur;
            lastShoot = Time.time;

            GetWeaponStands();
        }


        void GetWeaponStands()
        {
            weaponStands = new List<Transform>();

            foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
                if (t.gameObject.name.Contains("Stand"))
                    weaponStands.Add(t); }

            if (weaponStands.Count == 0)
                Debug.Log("Weapon has no stand");
        }


        void SetWeaponToHolder()
        {
            if (weaponHolder.tag == "Player") {
                SetupForFpsPlayer(); }

            //if (weaponHolder.tag == "Unit") {
            //    SetupForUnit(); }

            weaponHolder.GetComponent<IWeaponizeable>().EquipWeapon(weapon);
            anim.SetBool("IsWeaponized", true);
        }


        void SetupForFpsPlayer()
        {
            //Get FPS stands
            foreach (Transform t in weaponHolder.GetComponentsInChildren<Transform>()) {
                switch (t.gameObject.name) {
                    case "ShoulderStand": 
                        FPSShoulderStand = t;
                        break;
                    case "SideStand": 
                        FPSSideStand = t;
                        break;
                    case "FrontStand": 
                        FPSFrontStand = t;
                        break;
                }
            }
        
            //Find which FPS stand we use for the weapon
            foreach (Transform t in weaponStands) {
                switch (t.gameObject.name) {
                    case "ShoulderStand":
                        targetStand = FPSShoulderStand;
                        break;
                    case "SideStand": 
                        targetStand = FPSSideStand;
                        break;
                    case "FrontStand": 
                        targetStand = FPSFrontStand;
                        break;
                }
            }

            //Parent weapon to stand
            weapon.transform.parent = targetStand;
            weapon.transform.position = targetStand.position;
            weapon.transform.rotation = targetStand.rotation;
        }


        void SetupForUnit()
        {
            //Get Unit stand
            foreach (Transform t in weaponHolder.GetComponentsInChildren<Transform>()) {
                    if(t.name == "WeaponStand")
                       UnitStand = t; }
            
            //Parent weapon to stand
            weapon.transform.parent = UnitStand;
            weapon.transform.position = UnitStand.position;
            weapon.transform.rotation = UnitStand.rotation;
        }


        public void Shoot(bool AltShoot)
        {
            int ammo;
            float cadence;

            if (!AltShoot)
            {
                ammo = ShootAmmo;
                cadence = CadenceDeTir;
            } else {
                ammo = ShootAltAmmo;
                cadence = CadenceDeTirAlt;
            }

            //Shoot conditions
            if (chargeur < ammo) return;
            if ((Time.time - lastShoot) < cadence) return;

            CancelInvoke("Reload");

            chargeur -= ammo;
            lastShoot = Time.time;

            if (!AltShoot) OnShoot();
            if (AltShoot) OnShootAlt();

            if (chargeur != TailleChargeur) StartReload(); 
            //Debug.Log($"{gameObject.name} has {chargeur} munitions left");
        }


        public void StartReload()
        {
            //Debug.Log($"{gameObject.name} is reloading");
            Invoke("Reload", DureeRechargement);

        }


        void Reload()
        {
            chargeur += QteRechargement;

            if (chargeur < TailleChargeur)
                StartReload(); 
            else 
                chargeur = TailleChargeur;

            //Debug.Log($"{gameObject.name} has reloaded");
        }


        public abstract void OnShoot();
        public virtual void OnShootAlt() {}
    }
}
