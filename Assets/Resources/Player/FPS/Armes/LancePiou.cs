using UnityEngine;
using AbstractClasses;

public class LancePiou : Arme
{	
    public GameObject projectile;
    public int projectileSpeed;

	Camera cam;
    Transform gunPoint;
    Vector3 pos;
    Quaternion rot;
    Transform tr;

	void Start()
    {
        tr = new GameObject().transform;
        cam =  GetComponentInChildren<Camera>();
        gunPoint = cam.transform.Find("GunPoint");
    }
	
	public override void OnShoot()
    {
        tr.position = gunPoint.position;
        tr.rotation = cam.transform.rotation;
        SpawnManager.SpawnProjectile(projectile, tr.position, tr.rotation, projectileSpeed);
    }
	
    public override void OnShootAlt()
    {
        //Spawn Middle projectile
        SpawnManager.SpawnProjectile(projectile, gunPoint.transform.position, cam.transform.rotation, projectileSpeed);

        //Spawn Around projectiles
        for (int i = 0; i < 6; i++)
        {
            tr.position = gunPoint.transform.position;
            tr.rotation = cam.transform.rotation;

            tr.localEulerAngles += new Vector3(0, 0, 360/6 * i);
            tr.position += tr.TransformVector(new Vector3(1, 0, 0));
            tr.Rotate(new Vector3(0,1,0), 5f);

            SpawnManager.SpawnProjectile(projectile, tr.position, tr.rotation, projectileSpeed);
        }

    }

}
