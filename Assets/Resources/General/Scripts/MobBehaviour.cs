using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;




public class MobBehaviour : NetworkBehaviour
{
	SpawnManager SM;
	Transform target;
    Vector3 destination;
	NavMeshAgent agent;
	
	public GameObject explosion;
	
	
    // Start is called before the first frame update
	public override void OnNetworkSpawn()
    {
		SM = GetComponent<SpawnManager>();
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
				target = PlayerList.PlayerListinstance.GetPlayerObject(0).transform;
			}catch{}	
		
		}
		
		if(target!=null && Vector3.Distance(transform.position, target.position) < 2.50f)
		{
			//GameObject go=Instantiate(explosion,transform.position,Quaternion.identity);
			SM.Spawn(explosion.name,transform.position,Quaternion.identity);
			Destroy(gameObject);
		}
    }
}
