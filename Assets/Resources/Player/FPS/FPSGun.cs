using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FPSGun : MonoBehaviour
{	

    public GameObject projectile;

	int layerMask = 1 << 9;

	Camera cam;
	Rigidbody rb;
	RaycastHit hit;
	GameObject go;
    Transform gunPoint;
    Vector3 pos;
    Quaternion rot;

	void Start()
    {
        cam =  GetComponentInChildren<Camera>();
        gunPoint = transform.Find("GunPoint");
    }
	
	void Update()
	{
		if (Input.GetMouseButtonDown(0)) { CreateProjectile(); }
		if (Input.GetMouseButtonDown(1)) { Shoot(); }
	}
	
	void CreateProjectile()
    {
        pos = gunPoint.position;
        rot = cam.transform.rotation;
        SpawnManager.SpawnObject(projectile, pos, rot);
    }
	
	void Shoot()
	{
		Ray ray = cam.ScreenPointToRay( new Vector3(Screen.width /2, Screen.height /2, 0));
		if (Physics.Raycast(ray, out hit, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
	}

}
