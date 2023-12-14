using UnityEngine;
using classes;

public class LancePiou : Arme
{	
    public GameObject projectile;
    public int projectileSpeed;

	Camera cam;
    Transform gunpoint, tr;

    void Awake()
    {
        cam =  GetComponentInChildren<Camera>();
        gunpoint = transform.Find("Gunpoint");
        tr = new GameObject().transform;
    }

    void OnDisable()
    {
        Destroy(tr.gameObject);
    }

	public override void OnShoot()
    {
        SpawnManager.SpawnProjectile(projectile, gunpoint.transform.position, gunpoint.transform.rotation, projectileSpeed);
    }
	
    public override void OnShootAlt()
    {
        //Spawn Middle projectile
        SpawnManager.SpawnProjectile(projectile, gunpoint.transform.position, gunpoint.transform.rotation, projectileSpeed);

        //Spawn Around projectiles
        for (int i = 0; i < 6; i++)
        {
            tr.position = gunpoint.transform.position;
            tr.rotation = gunpoint.transform.rotation;

            tr.localEulerAngles += new Vector3(0, 0, 360/6 * i);
            tr.position += tr.TransformVector(new Vector3(1, 0, 0));
            tr.Rotate(new Vector3(0,1,0), 5f);

            SpawnManager.SpawnProjectile(projectile, tr.position, tr.rotation, projectileSpeed);
        }

    }

}
