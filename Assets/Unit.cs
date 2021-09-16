using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    //run to a point
    //necesitte un navmeshagent
    private NavMeshAgent nav;

    private void Start() 
    {
        nav =GetComponent<NavMeshAgent>();
    }    

    public void MoveToPosition(Vector3 pos)
    {
        nav.SetDestination(pos);
    }
}
