using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelections : StateMachineBehaviour
{
    public List<GameObject> selections = new List<GameObject>();

    RaycastHit hit;

    public void UnitClickSelect()
    {

    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetMouseButtonUp(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 400.0f, (1 << 9)))
            {

                if (hit.transform.root.CompareTag("Unit"))
                {
                    Debug.Log("unit selected");
                    //on click sur une unité
                    // on envoie la cible puis on passe a building selected state
                    // animator.GetBehaviour<UnitSelectedRTSState>().GetTarget(hit.transform.root.gameObject);

                    animator.SetBool("UnitSelection", true);
                }
            }
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
