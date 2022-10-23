using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class CubedelamortScript : NetworkBehaviour
{
	Vector3 push = new Vector3(0,0,2000);
	Rigidbody rb ;
	
    // Start is called before the first frame update
	public override void NetworkStart()
    {
		if (IsOwner) {enabled=false;}
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
			}
        
        
    }
}
