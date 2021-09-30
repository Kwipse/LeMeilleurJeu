using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectedRTSState : StateMachineBehaviour
{
    /*
    actions : appuyer echap pour sortir
    */
    public GameObject cible;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //reset des variables
        animator.SetBool("BuildingSelection",false );
            
           
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //on peut revenir en idle
        
        if(Input.GetKeyDown(KeyCode.A))
        {
            animator.SetBool("endSelection",true);
        }
        //oon peut créer une unité
        if(Input.GetKeyDown(KeyCode.W))
        {
            
            //on appele le building manager
            cible.GetComponent<Building_manager>().CreateMob();

        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cible = null ;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    public void GetTarget(GameObject go)
    {
        cible=go;
        Debug.Log("cible = "+cible);
    }
}
