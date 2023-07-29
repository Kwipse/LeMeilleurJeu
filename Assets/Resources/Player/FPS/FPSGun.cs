using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;


using UnityEngine;


public class FPSGun : MonoBehaviour

{	

	ulong localId;
	int layerMask = 1 << 9;
	SpawnManager SM;
	Camera cam;
	Rigidbody RBody;
	RaycastHit hit;
	GameObject go;
	Vector3 shootPositionCorrection = new Vector3(0,1,1);

	public GameObject cube;
	
	void Awake()
	{
		
			//Init Client ID
			localId = NetworkManager.Singleton.LocalClientId;
			//Init Things
			SM = GetComponent<SpawnManager>();
			cam = GetComponentInChildren<Camera>();
		
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
        //Debug.Log("cubeDeLaMort : " + cam.transform.Find("GunPoint").position + " - localId : "+localId);
		SM.Spawn("LeCubeDeLaMort",cam.transform.Find("GunPoint").position, localId, cam.transform.Find("GunPoint").rotation);
    }
	
	public void Shoot()
	{
		Ray ray = cam.ScreenPointToRay( new Vector3(Screen.width /2, Screen.height /2, 0));
		
		if (Physics.Raycast(ray, out hit, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
	}
}
