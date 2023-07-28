using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CubedelamortScript : NetworkBehaviour
{
	Vector3 push = new Vector3(0,0,500);
	Rigidbody rb ;
	NetworkRigidbody networkRigidbody;

    public void Start()
    {
        networkRigidbody = GetComponent<NetworkRigidbody>();

    }

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
		if (!IsOwner) {enabled=false;
			//Debug.Log("is Owner : " + IsOwner);
            addRelativeForceServerRpc(push); //destroy with 10s delay ?
			Debug.Log("LocalId non owner: " + NetworkManager.Singleton.LocalClientId);
        }
        else
		{
            Debug.Log("LocalId owner: " + NetworkManager.Singleton.LocalClientId);

            rb = GetComponent<Rigidbody>();
			//rb.AddRelativeForce(push);
            Debug.Log(rb);

            addRelativeForceServerRpc( push); //destroy with 10s delay ?

            //Destroy(gameObject,10);
		}
    }

	[ServerRpc(RequireOwnership =false)]
	private void addRelativeForceServerRpc( Vector3 push)
	{
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddRelativeForce(push);

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
				//Destroy(gameObject);
				DestroyCubeServerRpc();

        }
    }

	[ServerRpc(RequireOwnership =false)]
	private void DestroyCubeServerRpc(int dureeOuiNon =0 )
	{
		//GetComponent<Netw>
		if (dureeOuiNon == 0)
		{
			Destroy(gameObject);
		}else
			Destroy(gameObject,10);

    }
}
