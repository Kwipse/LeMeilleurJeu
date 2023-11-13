using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningSbire : StateMachineBehaviour
{
    /*
     * etat:mine non assignée, en route vers la mine
     * en train de miner,retour vers le nexus,déchargement
     * 
     *cherche une mine mine position
     *nexus position
     */
    public GameObject nexus, mine;
    public RTSRessourceManager _rM;
    public int loadCapacity = 10, load = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name == "Nexus")
            {
                nexus = obj;
            }
            if (obj.name == "GoldMine")
            {
                mine = obj;
            }
            if (obj.name == "RTSManager")
            {
               _rM = obj.GetComponent<RTSRessourceManager>();
            }
        }            
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
