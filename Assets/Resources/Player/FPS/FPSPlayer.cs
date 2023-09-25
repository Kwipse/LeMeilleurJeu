using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FPSPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponent<FPSCamera>().enabled = false;
            GetComponent<FPSMovement>().enabled = false;
            GetComponent<FPSJump>().enabled = false;
            GetComponent<FPSGun>().enabled=false;

        }

    }
 
    void Start()
    {
        ColorManager.SetObjectColors(gameObject);
    }
}
