using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations;


[CreateAssetMenu]
public class AnimationSystem : ScriptableObject
{
    public RuntimeAnimatorController rac;

    public AnimationClip aIdle;
    public AnimationClip aRunning;
    public AnimationClip aFalling;
    public AnimationClip aJumping;

    GameObject unit;
    Animator anim; 
    WeaponSystem wb;
    Vector3 position;

    AnimatorOverrideController aoc;
    AnimationClip clip;
    Motion motion;


    public void StartAnimations(GameObject unitToAnimate, WeaponSystem weaponSystem = null) 
    {
        unit = unitToAnimate;
        wb = weaponSystem;
        anim = unitToAnimate.GetComponent<Animator>();
        if (!anim) { anim = unitToAnimate.AddComponent<Animator>(); }

        anim.runtimeAnimatorController = rac;
        aoc = new AnimatorOverrideController(rac);

        aoc["Ground Idle"] = aIdle; //Always need Idle animation
        aoc["Running"] =  aRunning.empty ? aoc["Ground Idle"] : aRunning ; //Running default to Idle
        aoc["Falling"] =  aFalling.empty ? aoc["Ground Idle"] : aFalling ; //Falling default to Idle
        aoc["Jumping"] =  aJumping.empty ? aoc["Ground Idle"] : aJumping ; //Jumping default to Idle

        anim.runtimeAnimatorController = aoc;

        SetBool("IsGroundUnit", true);
        SetBool("IsFlyingUnit", false);
        SetBool("IsMoving", false);
        SetBool("IsMovingUp", false);
        SetBool("IsMovingDown", false);
    }


    //Call this in implementing class fixed update
    public void UpdateAnimation()
    {
        UpdateMovementAnimations();
        UpdateWeaponIK();
    }

    public void UpdateMovementAnimations()
    {
        Vector3 moveDelta = unit.transform.position - position;
        SetBool("IsMoving", Vector3.SqrMagnitude(moveDelta) > 0.05f); 
        SetBool("IsMovingDown", moveDelta.y < -0.05f, "OnMovingDown", "OnGrounded");
        SetBool("IsMovingUp", moveDelta.y > 0.05f, "OnMovingUp"); 
        position = unit.transform.position;
    }

    public void UpdateWeaponIK()
    {
        wb?.UpdateHandlesIK();
    }



    public void SetUnitMode(string mode) 
    {
        switch (mode)
        {
            case "ground":
                anim.SetBool("IsGroundUnit", true);
                anim.SetTrigger("OnSetUnitAsGrounded");
                break;

            case "flying":
                anim.SetBool("IsFlyingUnit", true);
                anim.SetTrigger("OnSetUnitAsFlying");
                break;
        }
    }


    //Set bool, and fire trigger if value changed
    void SetBool(string boolName, bool boolValue, string falseToTrueTrigger = null, string trueToFalseTrigger = null)
    {
        if (anim.GetBool(boolName) != boolValue)
        {
            anim.SetBool(boolName, boolValue);
            //Debug.Log($"{boolName} set to {boolValue}");
            if ((boolValue ? falseToTrueTrigger : trueToFalseTrigger) != null) 
            {
                anim.SetTrigger(boolValue ? falseToTrueTrigger : trueToFalseTrigger); 
                //Debug.Log($"firing {(boolValue ? falseToTrueTrigger : trueToFalseTrigger)} trigger");
            }
        }
    }

}


