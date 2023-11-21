using UnityEngine;
using Unity.Netcode;

namespace AbstractClasses {

    public abstract class Arme : NetworkBehaviour
    {
        public float CadenceDeTir;
        public int ShootAmmo;
        public float CadenceDeTirAlt;
        public int ShootAltAmmo;

        public float DureeRechargement;
        public int QteRechargement;
        public int TailleChargeur;

        float lastShoot;
        int chargeur;


        void Awake()
        {
            chargeur = TailleChargeur;
            lastShoot = Time.time;
        }

        void Update()
        {
            PlayerInputs();
        }


        void PlayerInputs()
        {
            if (Input.GetMouseButton(0)) Shoot(false, ShootAmmo, CadenceDeTir);
            if (Input.GetMouseButton(1)) Shoot(true, ShootAltAmmo, CadenceDeTirAlt);
            if (Input.GetKeyDown(KeyCode.R)) StartReload(); 
        }


        void Shoot(bool AltShoot, int ammo, float cadence)
        {
            //Shoot conditions
            if (chargeur < ammo) return;
            if ((Time.time - lastShoot) < cadence) return;

            chargeur -= ammo;
            lastShoot = Time.time;

            if (!AltShoot) OnShoot();
            if (AltShoot) OnShootAlt();

            if (chargeur == 0) StartReload(); 
            Debug.Log($"{gameObject.name} has {chargeur} munitions left");
        }



        void StartReload()
        {
            Debug.Log($"{gameObject.name} is reloading");
            Invoke("Reload", DureeRechargement);

        }


        void Reload()
        {
            chargeur += QteRechargement;

            if (chargeur < TailleChargeur)
                StartReload(); 
            else 
                chargeur = TailleChargeur;

            Debug.Log($"{gameObject.name} has reloaded");
        }


        public abstract void OnShoot();
        public virtual void OnShootAlt() {}
    }
}
