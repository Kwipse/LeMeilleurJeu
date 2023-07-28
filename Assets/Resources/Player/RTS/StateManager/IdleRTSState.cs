using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRTSState : StateMachineBehaviour
{   

/*
        Le layer 9 doit etre dédié aux clickables
        mettre les tag building sur les batiments 
*/
/*
    Cet état est le nexus entre les different etat de controle du joueur RTS
*/
    RaycastHit hit;
    public List<GameObject> selection = new List<GameObject>();

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entering IdleMode");
        //on reset les variables internes de l'état précedent
        animator.SetBool("IsConstructed", false);
        animator.SetBool("endSelection", false); 
        animator.SetBool("isMineConstructing", false);
        animator.SetBool("mineConstructionEnd", false);
        animator.SetBool("mobCreationEnd", false); 
        animator.SetBool("sbireCreationEnd", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //on défini la capacité à clicker sur un batiment ou une unité
        /*
         * 
         */

        if(Input.GetMouseButtonUp(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, 400.0f, (1<<9)))
            {
   
     

                if( hit.transform.root.CompareTag("Building"))
                {
                    Debug.Log("building selected");
                    //on click sur un batiment
                    // on envoie la cible puis on passe a building selected state
                    animator.GetBehaviour<BuildingSelectedRTSState>().GetTarget(hit.transform.root.gameObject);
                   
                    animator.SetBool("BuildingSelection", true);
                }
                else if( hit.transform.root.CompareTag("Unit"))
                {
                    Debug.Log("unit selected");
                    //on click sur une unité
                    // on envoie la cible puis on passe a building selected state
                   // animator.GetBehaviour<UnitSelectedRTSState>().GetTarget(hit.transform.root.gameObject);
                   
                    animator.SetBool("UnitSelection", true);
                }
            }
        }
        //
        //sbire a, mine z 
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("mobCreation", true);

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetBool("isMineConstructing", true);

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetBool("sbireCreation", true);

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
