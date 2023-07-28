using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class CubedelamortScript : NetworkBehaviour
{
	Vector3 push = new Vector3(0,0,500);
	Rigidbody rb ;
	
    // Start is called before the first frame update
	public override void OnNetworkSpawn()
    {
		if (!IsOwner) {enabled=false;}
		else
		{
			rb= GetComponent<Rigidbody>();
			rb.AddRelativeForce(push);
			Destroy(gameObject,10);
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
				Destroy(gameObject);
            DestroyCubeServerRpc();

        }
        if (collision.gameObject.tag == "Building")
			{
				collision.collider.GetComponent<HealthSystem>().LoosePv(100);
				//Destroy(gameObject);
				DestroyCubeServerRpc();

            }
			if(collision.gameObject.tag == "Player")
			{
				collision.collider.GetComponent<HealthSystem>().LoosePv(25);
				Destroy(gameObject);
            DestroyCubeServerRpc();

        }
    }

	[ServerRpc]
	private void DestroyCubeServerRpc( )
	{
		//GetComponent<Netw>

		Destroy(gameObject);

	}
}
