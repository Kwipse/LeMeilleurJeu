using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;


    public class FPSPlayer : NetworkBehaviour
    {



       public override void NetworkStart()
       {

            if (!IsOwner)
            {
			
				GetComponent<FPSMovement>().enabled = false;
				GetComponent<FPSJump>().enabled = false;

            }
            
       }
    }
