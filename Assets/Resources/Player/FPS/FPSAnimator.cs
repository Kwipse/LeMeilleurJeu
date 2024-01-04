using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;
using classes;


public class FPSAnimator : NetworkBehaviour 
{
    [HideInInspector]
    public GameObject currentWeapon;
    
	Animator Anim;
	Rigidbody RBody;
	Vector3 position;
    bool wasFalling;
    bool wasMovingUp;
    bool wasWeaponized;

    GameObject player;
    FPSPlayer playerScript;
    Transform playerLeftHand;
    Transform playerRightHand;
    TwoBoneIKConstraint rightHandConstraint;
    TwoBoneIKConstraint leftHandConstraint;

    Transform weaponLeftHandle;
    Transform weaponRightHandle;

	void Awake()
    {
		Anim = GetComponent<Animator>();
		RBody = GetComponent<Rigidbody>();	
        playerScript = GetComponent<FPSPlayer>();

        //Init Hands IK
        foreach (TwoBoneIKConstraint c in GetComponentsInChildren<TwoBoneIKConstraint>()) {
            if (c.gameObject.name == "MainDroiteIK") {
                rightHandConstraint = c;
                playerRightHand = c.gameObject.transform.Find("MainDroiteIK_target");
                //Debug.Log($"Player right hand : {playerRightHand.position}");
            }
            if (c.gameObject.name == "MainGaucheIK") {
                leftHandConstraint = c;
                playerLeftHand = c.gameObject.transform.Find("MainGaucheIK_target"); 
                //Debug.Log($"Player left hand : {playerLeftHand.position}");
            }
        }
    }

    void Start()
    {
        Anim.SetBool("IsMoving", false);
        Anim.SetBool("IsFalling", false);
        wasFalling = false;
        wasMovingUp = false;
        position = RBody.position;

        currentWeapon = null;
    }

	void FixedUpdate()
	{
        UpdateStates();
        UpdateHandsIK();
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

    public void UpdateCurrentWeapon()
    {
        currentWeapon = playerScript.currentWeapon;
        weaponLeftHandle = currentWeapon.transform.Find("LeftHandle");
        weaponRightHandle = currentWeapon.transform.Find("RightHandle");

        //Debug.Log($"{currentWeapon.name} is being animated");
        Anim.SetBool("IsWeaponized", true);
    }


    void UpdateHandsIK()
    {
        if (Anim.GetBool("IsWeaponized"))
        {
            //Debug.Log($"Weapon IKs are being animated");

            if (weaponLeftHandle) {
                //Debug.Log($"leftHand : {playerLeftHand.position}");
                //Debug.Log($"leftHandle : {weaponLeftHandle.position}");
                leftHandConstraint.weight = 1;
                playerLeftHand.position = weaponLeftHandle.position;
                playerLeftHand.rotation = weaponLeftHandle.rotation; }
            else {
                leftHandConstraint.weight = 0;
            }


            if (weaponRightHandle) {
                rightHandConstraint.weight = 1;
                playerRightHand.position = weaponRightHandle.position;
                playerRightHand.rotation = weaponRightHandle.rotation; }
            else {
                rightHandConstraint.weight = 0;
            }

        }
    }
}
