using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

using UnityEngine;


    public class FPSPlayer : NetworkBehaviour
    {
		
		
		
		public override void OnNetworkSpawn()
		{

            if (!IsOwner)
            {
				//this.enabled = false;
				GetComponentInChildren<Camera>().enabled = false;
				//GetComponent<FPSCamera>().enabled = false;
				GetComponent<FPSMovement>().enabled = false;
				GetComponent<FPSJump>().enabled = false;
				GetComponent<FPSGun>().enabled=false;

            }
            
		}
    }
