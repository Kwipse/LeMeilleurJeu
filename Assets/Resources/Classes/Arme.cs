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
            weapon = gameObject;
            chargeur = TailleChargeur;
            lastShoot = Time.time;
        }


        //Fonctions a implementer dans vos scripts d'arme
        public abstract void OnShoot(); //obligatoire
        public virtual void OnShootAlt() {} //optionnel


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
    }
}
