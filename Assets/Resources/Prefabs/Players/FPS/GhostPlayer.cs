using UnityEngine;

[RequireComponent(typeof(FPSCamera))]

public class GhostPlayer : SyncedBehaviour, IWaitForGameSync
{
    public FPSMovement MV;
    void Awake()
    {
        enabled = false;
        MV = ScriptableObject.Instantiate(MV);
    }

    public override void StartAfterGameSync()
    {

        if (IsOwner)
        {
            MV.SetMovement(gameObject);
        }

        if (!IsOwner) {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<Canvas>().enabled = false;
        }

        enabled = true;
    }
    void Update()
    {
        if (IsOwner)
        {
            PlayerInputs(); 
        }
    }
    void FixedUpdate()
    {
        if (IsOwner) { MV.UpdatePosition(); }
    }

    void PlayerInputs()
    {
        if (Input.GetKey(KeyCode.Z)) { MV.MoveForward(); }
        if (Input.GetKey(KeyCode.Q)) { MV.MoveLeft(); }
        if (Input.GetKey(KeyCode.S)) { MV.MoveBackward(); }
        if (Input.GetKey(KeyCode.D)) { MV.MoveRight(); }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { MV.ToggleSprint(); }
    }




    public MovementSystem GetMovementSystem() { return MV; }

}
