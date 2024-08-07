using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementSystem : ScriptableObject
{
    public bool showMovementLogs = false;
    public float moveSpeed = 20;
    public float slideSpeed = 30;
     
    [HideInInspector] public Transform movingTr;
    [HideInInspector] public Rigidbody movingRb;
    [HideInInspector] public Collider movingCol;

    Vector3 moveVect = Vector3.zero;
    RaycastHit lastHit;
    float lastTime;

    GameObject currentGround, lastGround = null;
    bool isGrounded, wasGrounded = false;
    bool isGoingUp, wasGoingUp = false;
    bool snapToGround, lastSnap = false;

    public void SetMovement(GameObject go)
    {
        lastTime = Time.time;
        movingRb = go.GetComponent<Rigidbody>();
        movingCol = go.GetComponent<Collider>();
        movingTr = go.transform;

        movingRb.isKinematic = false;


        if (showMovementLogs) { Debug.Log($"MovementSystem set for {go.name}"); }
        OnSetMovement(go);
    }

    public abstract void OnSetMovement(GameObject movingGo);


    //Basic methods
    public void AddForce(Vector3 force) { movingRb.AddForce(force); } //todo? : tuer la vitesse verticale avant de forcer
    public void AddRelativeForce(Vector3 force) { movingRb.AddRelativeForce(force); }
    public void Move(Vector3 direction) { moveVect += direction; }
    public void Teleport(Vector3 position) { movingTr.position = position; }

    //Call this in FixedUpdate()'s implementing class 
    public void UpdatePosition()
    {
        float moveDelta = (Time.time - lastTime) / Time.fixedDeltaTime;
        float currentUpForce = movingRb.GetAccumulatedForce().y; 
        float currentYVelocity = movingRb.velocity.y;
        isGoingUp = (movingRb.velocity.y > 0.1f) || (currentUpForce > 0);


        //Raycasts under the object
        Ray ray = new Ray(movingTr.position, -Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit groundHit, 3000f))
        {
            currentGround = groundHit.collider.gameObject;
            isGrounded = currentUpForce > 0 ? false : groundHit.distance < 0.5f;
        }

        if (!wasGrounded)
        {
            if (isGoingUp) { snapToGround = false; }
            if (!isGoingUp) { snapToGround = isGrounded; }
        }


        if (wasGrounded)
        {
            float heightDiff = groundHit.distance - lastHit.distance;
            bool isChangingHeight = heightDiff > 0.5f;
            bool isChangingNormal = lastHit.normal != groundHit.normal; 

            //Ledge transition
            if (isChangingHeight) { snapToGround = false; }

            //Ground normal transition
            if (isChangingNormal && !isChangingHeight) {
                bool isConvex = (Vector3.ProjectOnPlane(moveVect, lastHit.normal) - Vector3.ProjectOnPlane(moveVect, groundHit.normal)).z > 0;
                if (isConvex) { snapToGround = true; }
                if (!isConvex) { snapToGround = movingRb.velocity.magnitude < slideSpeed; } }
        }


        //Override on applied force (ie: jump)
        if (currentUpForce != 0) { snapToGround = false; }


        isGrounded = snapToGround;




        //Update velocity
        float newYVelocity = snapToGround ? 0 : movingRb.velocity.y;
        moveVect = snapToGround ? Vector3.ProjectOnPlane(moveVect, groundHit.normal) : moveVect;
        movingRb.velocity = (moveVect.normalized * moveSpeed * moveDelta) + new Vector3(0, newYVelocity, 0);
        movingRb.angularVelocity = Vector3.zero; 


        //showlog();

        //Fire grounded virtual methods
        if (wasGrounded != isGrounded)
        {
            if (isGrounded) { OnGettingToGround(groundHit); }
            if (!isGrounded) { OnLeavingGround(groundHit); }
        }

        //Misc
        showlog();
        Debug.DrawRay(movingTr.position, movingRb.velocity, Color.red);
        lastTime = Time.time;
        moveVect = Vector3.zero;

        wasGrounded = isGrounded;
        wasGoingUp = isGoingUp;
        lastSnap = snapToGround;
        lastGround = currentGround;
        lastHit = groundHit;
    }

    public virtual void OnGettingToGround(RaycastHit hit) { }
    public virtual void OnLeavingGround(RaycastHit hit) { }

    void showlog()
    {
        if (!showMovementLogs) { return; }
        if (!currentGround || !lastGround) { return; }

        if ((lastGround != currentGround) || (wasGrounded != isGrounded) || (lastSnap != snapToGround) || (wasGoingUp != isGoingUp))
        {
            Debug.Log($"{movingRb.gameObject.name} | Grounded : {wasGrounded}->{isGrounded} | Snap : {lastSnap}->{snapToGround} | Up : {wasGoingUp}->{isGoingUp} | {lastGround.name}->{currentGround.name} | {movingRb.velocity}");
        }

    }
}
