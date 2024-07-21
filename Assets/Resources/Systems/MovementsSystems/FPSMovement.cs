using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FPSMovement : MovementSystem
{
    public int nbJumpMax;
    public float jumpForce;
    int nbJump = 0;


    //Common functions
    public void MoveForward() { Move(movingTr.forward); }
    public void MoveBackward() { Move(-movingTr.forward); }
    public void MoveLeft() { Move(-movingTr.right); }
    public void MoveRight() { Move(movingTr.right); }

    public void Jump() {
        if (nbJumpMax > nbJump ) {
            AddForce(new Vector3(0, jumpForce, 0));
            nbJump += 1; } }

    public void RechargeJumps() {
        nbJump = 0; }
}

