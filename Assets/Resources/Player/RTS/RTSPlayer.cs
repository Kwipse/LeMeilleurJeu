using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
	public override void OnNetworkSpawn()
	{
		if (!IsOwner) {

            GetComponentInChildren<Camera>().enabled = false;
            GetComponent<RTSCamera>().enabled = false;
            GetComponent<RTSSelection>().enabled = false;
            GetComponent<Animator>().enabled = false;
        }
	}
}


