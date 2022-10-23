using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;


public class MobBehaviour : NetworkBehaviour
{
	public GameObject explosion;
	Transform target;
    Vector3 destination;
	NavMeshAgent agent;
    // Start is called before the first frame update
	public override void NetworkStart()
    {
		agent = GetComponent<NavMeshAgent>();
        destination = agent.destination;   
	
    }
	
	
	
	
	
	
    // Update is called once per frame
    void Update()
    {
         if (target!=null && Vector3.Distance(destination, target.position) > 1.0f)
        {
            destination = target.position;
            agent.destination = destination;
        }
		if(target==null)
		{
			//Debug.Log(PlayerList.GetPlayerObject(1).transform);
			//TODO : checker si c'est GetPlayerObject(0) ou (1) ????
			try{
				target = PlayerList.GetPlayerObject(0).transform;
			}catch{}	
			//Debug.Log("playerObjectName: "+ PlayerList.GetPlayerObject(1)+"fin");
			//Debug.Log("mobbehaviour:"+ target);
		}
		
		if(target!=null && Vector3.Distance(transform.position, target.position) < 2.50f)
		{
			GameObject go=Instantiate(explosion,transform.position,Quaternion.identity);
			Destroy(gameObject);
		}
    }
}
