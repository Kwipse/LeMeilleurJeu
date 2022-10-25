using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;


public class FPSGun : NetworkBehaviour

{
	int layerMask = 1 << 9;
	RaycastHit hit;

	public GameObject cube; 

	ulong localId;
	SpawnManager SM;
	
	Rigidbody RBody;	
	
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
			SM = GetComponent<SpawnManager>();
			
		
			
		}
	}
	

	//if (Input.GetMouseButtonDown(0)) {SM.Spawn("LeCubeDeLaMort",transform.localPosition,localId);}
	
	void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
		CreateBullet();
		}
		if (Input.GetMouseButtonDown(01)) {
		Shoot();
		}
		
	}
	
	public void CreateBullet()
    {
        //GameObject go = Instantiate(cube,Camera.main.transform.Find("GunPoint").position,Camera.main.transform.rotation);
        Debug.Log("cubeDeLaMort : " + Camera.main.transform.Find("GunPoint").position + " - localId : "+localId);
		SM.Spawn("LeCubeDeLaMort",Camera.main.transform.Find("GunPoint").position, localId, Camera.main.transform.Find("GunPoint").rotation);
    }
	
	public void Shoot()
	{
		Ray ray = Camera.main.ScreenPointToRay( new Vector3(Screen.width /2, Screen.height /2, 0));
		
		if (Physics.Raycast(ray, out hit, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
	}
}
