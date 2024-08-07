using UnityEngine;
using Unity.Netcode;


using scriptablesobjects;


public class FPSAnimator : NetworkBehaviour 
{
	Animator Anim;
	Rigidbody RBody;
	Vector3 position;
    bool wasFalling;
    bool wasMovingUp;
    bool wasWeaponized;

    GameObject player;
    FPSPlayer playerScript;
    WeaponSystem WS;

	void Awake() 
    {
		Anim = GetComponent<Animator>();
		RBody = GetComponent<Rigidbody>();	
        playerScript = GetComponent<FPSPlayer>();
    }

    void Start() 
    {
        WS = GetComponent<WeaponSystem>();
        Anim.SetBool("IsMoving", false);
        Anim.SetBool("IsFalling", false);
        wasFalling = false;
        wasMovingUp = false;
        position = RBody.position; 
    }

	void FixedUpdate() 
    {
        UpdateStates(); 
        WS.UpdateHandlesIK();
    }
	

    void UpdateStates()
    {
        //set new states based on object movement
        Vector3 moveDelta = RBody.position - position;
        Anim.SetBool("IsMoving", Vector3.SqrMagnitude(moveDelta) > 0.01f); 
        Anim.SetBool("IsFalling", moveDelta.y < -0.05);
        Anim.SetBool("IsMovingUp", moveDelta.y > 0.05);
        
        //fire triggers
        if ((!wasFalling) && (Anim.GetBool("IsFalling")))
                Anim.SetTrigger("OnFalling");
        if ((!wasMovingUp) && (Anim.GetBool("IsMovingUp")))
                Anim.SetTrigger("OnJump");

        //misc
        wasFalling = Anim.GetBool("IsFalling");
        wasMovingUp = Anim.GetBool("IsMovingUp");
        position = RBody.position;
    }
}
