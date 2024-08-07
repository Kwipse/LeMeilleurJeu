using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FPSMovement : MovementSystem
{
    public int nbJumpMax;
    public float jumpForce;
    int nbJump = 0;
    bool sprint = false;

    public override void OnSetMovement(GameObject movingGo)
    {

    }

    public override void OnGettingToGround(RaycastHit hit)
    {
        nbJump = 0;
    }

    public void MoveForward() { Move(movingTr.forward); }
    public void MoveBackward() { Move(-movingTr.forward); }
    public void MoveLeft() { Move(-movingTr.right); }
    public void MoveRight() { Move(movingTr.right); }

    public void Jump() {
        if (nbJumpMax > nbJump ) {
            AddForce(new Vector3(0, jumpForce * 1000, 0));
            nbJump += 1; } }

    public void ToggleSprint()
    {
        sprint = !sprint;
        if (sprint) { moveSpeed *= 2; }
        if (!sprint) { moveSpeed /= 2; }
    }

}

