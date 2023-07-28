using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadingMineState : StateMachineBehaviour
{
    private MiningSbire mS;
    private RTSRessourceManager _rM;
    private float nextTick = 0.0f;
    private float tickRate = 0.3f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isUnloading", false);
        mS = animator.GetBehaviour<MiningSbire>();
        _rM = mS._rM;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Time.time >= nextTick)
        {
            nextTick = Time.time + tickRate;
            mS.load--;
            _rM.AddGold(1);

        }
        if (mS.load ==0)
        {
            animator.SetBool("isGoingToMine", true);
        }
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
}
