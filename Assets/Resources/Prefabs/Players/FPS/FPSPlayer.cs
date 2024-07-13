using UnityEngine;
using Unity.Netcode;
using scriptablesobjects;

[RequireComponent(typeof(FPSCamera))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(WeaponSystem))]

public class FPSPlayer : SyncedBehaviour, IWaitForGameSync, IWeaponizable//, IHealth
{
    public FPSUI UI;
    public FPSMovement MV;
    public AnimationSystem AS;
    [field:SerializeField] public WeaponSystem WS {get; set;}
    //[field:SerializeField] public HealthSystem_wip HS {get; set;}
    HealthSystem HS;



    NetworkObject no;

    void Awake()
    {
        enabled = false;
        AS = ScriptableObject.Instantiate(AS);
        UI = ScriptableObject.Instantiate(UI);
        //WS = ScriptableObject.Instantiate(WS);
        //HS = ScriptableObject.Instantiate(HS);
        WS = GetComponent<WeaponSystem>();
        HS = GetComponent<HealthSystem>();
        MV = ScriptableObject.Instantiate(MV);
    }

    public override void StartAfterGameSync()
    {
        ColorManager.SetObjectColors(gameObject);
        AS.StartAnimations(gameObject, WS);
        //WS.StartWeaponSystem(gameObject);
        //HS.Init(gameObject);

        if (IsOwner)
        {
            UI.SetUI(gameObject);
            MV.SetMovingObject(gameObject);
        }

        if (!IsOwner) {
            GetComponentInChildren<Camera>().enabled = false; }

        enabled = true;
    }

    void Update()
    {
        if (IsOwner)
        {
            PlayerInputs(); 
            MV.UpdatePosition();
        }
    }

    void FixedUpdate()
    {
        AS.UpdateAnimation();
    }

    public override void OnDestroy()
    {
        Destroy(WS);
        base.OnDestroy();
    }

    void PlayerInputs()
    {
        if (Input.GetMouseButton(0)) WS.ShootWeapon();
        if (Input.GetMouseButton(1)) WS.ShootAltWeapon();
        if (Input.GetKeyDown(KeyCode.R)) WS.ReloadWeapon(); 
        if (Input.GetKeyDown(KeyCode.T)) WS.GetCurrentBackpackAmmo().SetAmmoToFull(); 
        if (Input.mouseScrollDelta.y > 0) WS.EquipNextWeapon();
        if (Input.mouseScrollDelta.y < 0) WS.EquipPreviousWeapon();

        if (Input.GetKeyDown(KeyCode.Space)) MV.Jump();
        if (Input.GetKey(KeyCode.Z)) MV.MoveForward();
        if (Input.GetKey(KeyCode.Q)) MV.MoveLeft();
        if (Input.GetKey(KeyCode.S)) MV.MoveBackward();
        if (Input.GetKey(KeyCode.D)) MV.MoveRight();
    }


    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "ground") {
            MV.RechargeJumps(); } }

    public AnimationSystem GetAnimationSystem() { return AS; }
    public WeaponSystem GetWeaponSystem() { return WS; }
    public MovementSystem GetMovementSystem() { return MV; }
    public HealthSystem GetHealthSystem() { return HS; }
    //public HealthSystem_wip GetHealthSystem() { return HS; }
}
