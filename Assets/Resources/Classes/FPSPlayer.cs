using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using interfaces;

namespace classes {

    [RequireComponent(typeof(FPSCamera))]
    [RequireComponent(typeof(FPSMovement))]
    [RequireComponent(typeof(FPSAnimator))]

    public class FPSPlayer : NetworkBehaviour, IWeaponizeable
    {
        public List<GameObject> ArmesDisponibles;

        [HideInInspector] public GameObject currentWeapon;

        Arme weaponScript;
        Vector3 gunPoint;
        FPSAnimator anim;
        int currentWeaponNumber;

        void Awake()
        {
            anim = GetComponent<FPSAnimator>();
        }

        public override void OnNetworkSpawn()
        {
            ColorManager.SetObjectColors(gameObject);

            if (!IsOwner)
            {
                GetComponentInChildren<Camera>().enabled = false;
                GetComponent<FPSCamera>().enabled = false;
                GetComponent<FPSMovement>().enabled = false;
            }
        }

        void Start()
        {
            SelectWeapon(0);
        }

        void Update()
        {
            PlayerInputs();
        }

        void PlayerInputs()
        {
            if (Input.GetMouseButton(0)) weaponScript.Shoot(false);
            if (Input.GetMouseButton(1)) weaponScript.Shoot(true);
            if (Input.GetKeyDown(KeyCode.R)) weaponScript.StartReload(); 
            if (Input.mouseScrollDelta.y > 0) SelectWeapon(currentWeaponNumber + 1);
            if (Input.mouseScrollDelta.y < 0) SelectWeapon(currentWeaponNumber - 1);
        }


        void SelectWeapon(int weaponNumber)
        {
            GameObject weaponToEquip;

            //Choisis l'arme
            if (weaponNumber > ArmesDisponibles.Count - 1)
                weaponToEquip = ArmesDisponibles[0];
            else if (weaponNumber < 0)
                weaponToEquip = ArmesDisponibles[ArmesDisponibles.Count - 1];
            else
                weaponToEquip = ArmesDisponibles[weaponNumber];

            //Remplace l'arme
            currentWeaponNumber = ArmesDisponibles.IndexOf(weaponToEquip);
            if (currentWeapon)
                SpawnManager.DestroyObject(currentWeapon);
            SpawnManager.SpawnWeapon(weaponToEquip, this.gameObject);
        }


        //Appelé par le script de l'arme
        public void EquipWeapon(GameObject weapon)
        {
            currentWeapon = weapon;
            weaponScript = currentWeapon.GetComponent<Arme>();
            anim.UpdateCurrentWeapon();
        }

    }

}
