using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class Unit : MonoBehaviour
{
    //necessite un navmeshagent
    private NavMeshAgent nav;
	public string type;

    private void Start() 
    {
        nav = GetComponent<NavMeshAgent>();
        GameObject.Find("RTSManager").GetComponent<UnitList>().AddUnit(gameObject);
		type = gameObject.name;
    }    

    //run to a point
    public void MoveToPosition(Vector3 pos)
    {
        nav.SetDestination(pos);
    }

    public void Stop()
    {
        nav.SetDestination(transform.position);
    }

    // a faire
    //void OnDestroy() { transform.root.gameObject.GameOject.Find("RTSManager").GetComponent<UnitList>().DelUnit(gameObject); }

}
