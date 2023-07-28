using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToTheMineState : StateMachineBehaviour
{
    private MiningSbire mS;
    private Unit _unit;
   
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mS = animator.GetBehaviour<MiningSbire>();
        _unit = animator.transform.GetComponent<Unit>();
       _unit.MoveToPosition(mS.mine.transform.position);
        animator.SetBool("isGoingToMine", false);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //check la distace à la mine
        if(Vector3.SqrMagnitude(animator.transform.position - mS.mine.transform.position) <30.0f)
        {
            animator.SetBool("isLoading", true);
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
