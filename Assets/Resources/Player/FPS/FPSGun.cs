using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;


public class FPSGun : NetworkBehaviour

{

	public GameObject cube; 

	ulong localId;
	SpawnManager SM;
	Animator Anim;	
	Rigidbody RBody;	
	
	Vector3 translation;
    Vector3 velocity;
	Vector3 shootPositionCorrection = new Vector3(0,1,1);

	GameObject go;
	
	public override void NetworkStart()
	{
		
		if (!IsOwner) {enabled = false; } //Disable for non-owner
		else
		{
			//Init Client ID
			localId = NetworkManager.Singleton.LocalClientId;
			
			//Init Things
			Anim = GetComponent<Animator>();
			SM = GetComponent<SpawnManager>();
			
			//Init PEWPEW
			go = Instantiate(cube);
			
			
		}
	}
	

	//if (Input.GetMouseButtonDown(0)) {SM.Spawn("LeCubeDeLaMort",transform.localPosition,localId);}
	
	void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
		CreateBullet();
		}
		//SM.Spawn("HumanBox",Vector3.zero,localId);
	}
	
	public void CreateBullet()
    {
        //GameObject go = Instantiate(cube,Camera.main.transform.Find("GunPoint").position,Camera.main.transform.rotation);
        Debug.Log("cubeDeLaMort : " + Camera.main.transform.Find("GunPoint").position + " - localId : "+localId);
		SM.Spawn("LeCubeDeLaMort",Camera.main.transform.Find("GunPoint").position,localId);
    }
}
