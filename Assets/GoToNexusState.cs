using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GoToNexusState : StateMachineBehaviour
{
    private MiningSbire mS;
    private Unit _unit;

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mS = animator.GetBehaviour<MiningSbire>();
        _unit = animator.transform.GetComponent<Unit>();
        _unit.MoveToPosition(mS.nexus.transform.position);
        animator.SetBool("isGoingToNexus", false);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //check la distace à la mine
        if (Vector3.SqrMagnitude(animator.transform.position - mS.nexus.transform.position) < 30.0f)
        {
            animator.SetBool("isUnloading", true);
            _unit.Stop();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
}
