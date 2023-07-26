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
			
				GetComponent<FPSMovement>().enabled = false;
				GetComponent<FPSJump>().enabled = false;
				GetComponent<FPSGun>().enabled=false;

            }
            
       }
    }
