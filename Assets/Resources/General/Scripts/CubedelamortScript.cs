using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CubedelamortScript : NetworkBehaviour
{
	Vector3 push = new Vector3(0,0,500);
	Rigidbody rb ;
	SpawnManager SM;
	NetworkObject no;
	ulong localId;
	
	public override void OnNetworkSpawn()
    {
		if (!IsServer) {enabled=false;}
		else
		{
			
			SM = GetComponent<SpawnManager>();
			no = GetComponent<NetworkObject>();
			rb= GetComponent<Rigidbody>();
			localId = NetworkManager.Singleton.LocalClientId;
			
			rb.AddRelativeForce(push);
			SM.DestroyCubeServerRpc(1);
			
			Debug.Log("POUNNNNLMMM : "+localId);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnCollisionEnter(Collision collision)
	{
            if(collision.gameObject.tag == "Unit")
			{
				collision.collider.GetComponent<HealthSystem>().LoosePv(100);
				//Destroy(gameObject);
				SM.DestroyCubeServerRpc();

        }
        if (collision.gameObject.tag == "Building")
			{
				collision.collider.GetComponent<HealthSystem>().LoosePv(100);
				//Destroy(gameObject);
				SM.DestroyCubeServerRpc();

            }
			if(collision.gameObject.tag == "Player")
			{
				collision.collider.GetComponent<FPSPlayerHealth>().LoosePv(25);
				//Destroy(gameObject);
				SM.DestroyCubeServerRpc();

        }
    }

	
}
