using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
	
	//Declarations
	ulong localId;
	Animator anim;
	SpawnManager SM;
	

	public override void OnNetworkSpawn()
	{
		
		if (!IsOwner) {enabled = false; } //Disable for non-owner
		else
		{
			//Init Client ID
			localId = NetworkManager.Singleton.LocalClientId;
			
			//Init Things
			anim = GetComponent<Animator>();
			SM = GetComponent<SpawnManager>();
		}
	}



	void Update()
	{
		KeyboardInputs();
		MouseInputs();
	}


	void KeyboardInputs()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// on se met en mode construction
			anim.SetBool("ConstructionParam", true);
		}
	}

	void MouseInputs()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Plane plane = new Plane(Vector3.up, Vector3.zero);

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


			if (plane.Raycast(ray, out float entry))
			{
			   SM.Spawn("HumanBox", ray.GetPoint(entry), localId, Quaternion.identity);
			}
		}
	}
	
	
}


