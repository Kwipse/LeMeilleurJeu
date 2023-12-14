using UnityEngine;
using Unity.Netcode;
using interfaces;

namespace classes {

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
        Transform weaponGunpoint, weaponHolderGunpoint;
        Animator anim;

        float lastShoot;
        int chargeur;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) 
                enabled = false;
        }

        public virtual void Start()
        {
            weapon = gameObject;
            anim = weaponHolder.GetComponent<Animator>();

            chargeur = TailleChargeur;
            lastShoot = Time.time;

            SetupWeapon();
        }

        void SetupWeapon()
        {
            //Debug.Log($"{weaponHolder.name} equips {weapon.name}");

            weaponHolderGunpoint = getGunpoint(weaponHolder);

            weapon.transform.parent = weaponHolderGunpoint;
            weapon.transform.position = weaponHolderGunpoint.position;
            weapon.transform.rotation = weaponHolderGunpoint.rotation;

            weaponHolder.GetComponent<IWeaponizeable>().EquipWeapon(weapon);
            anim.SetBool("IsWeaponized", true);
        }


        Transform getGunpoint(GameObject go) {
            foreach (Transform t in go.transform.GetComponentsInChildren<Transform>()) 
                if (t.gameObject.name == "Gunpoint") 
                    return t.gameObject.transform;
            return null; }


        void Update()
        {

        }

        public void Shoot(bool AltShoot)
        {
            int ammo;
            float cadence;

            if (!AltShoot)
            {
                ammo = ShootAmmo;
                cadence = CadenceDeTir;
            } else
            {
                ammo = ShootAltAmmo;
                cadence = CadenceDeTirAlt;
            }

            //Shoot conditions
            if (chargeur < ammo) return;
            if ((Time.time - lastShoot) < cadence) return;

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
