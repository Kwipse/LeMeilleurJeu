using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    //s'ajoute a la list des unités

    //run to a point
    //necesitte un navmeshagent
    private NavMeshAgent nav;

    private void Start() 
    {
        nav = GetComponent<NavMeshAgent>();
        GameObject.Find("RTSManager").GetComponent<UnitList>().AddUnit(gameObject);
    }    

    public void MoveToPosition(Vector3 pos)
    {
        nav.SetDestination(pos);
    }
    // a faire
    //void OnDestroy() { transform.root.gameObject.GameOject.Find("RTSManager").GetComponent<UnitList>().DelUnit(gameObject); }

}
