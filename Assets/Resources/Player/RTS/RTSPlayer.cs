using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEditor.Animations;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(RTSCamera))]
[RequireComponent(typeof(RTSSelection))]
[RequireComponent(typeof(Animator))]

public class RTSPlayer : NetworkBehaviour
{
    public AnimatorController RTSAnimator;

    public List<GameObject> AvailableBuildings;


    void Awake()
    {

    }

	public override void OnNetworkSpawn()
	{
		if (!IsOwner) {

            GetComponentInChildren<Camera>().enabled = false;
            GetComponent<RTSCamera>().enabled = false;
            GetComponent<RTSSelection>().enabled = false;
            GetComponent<Animator>().enabled = false;
        }
	}

    void Start()
    {
        GetComponent<Animator>().runtimeAnimatorController = RTSAnimator;
    }
}


