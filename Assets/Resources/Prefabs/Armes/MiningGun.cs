using UnityEngine;
using System.Collections;


public class MiningGun : Arme
{	
    Transform gunpoint;

    void Start()
    {
        gunpoint = GetGunpointTransform();
    }
	
	public override void OnShoot()
    {
        Ray ray = new Ray(gunpoint.position, gunpoint.forward); 
        if (Physics.Raycast(ray, out RaycastHit hit, 3000f))
        {
            EffectManager.LineEffect(gunpoint.position, hit.point, 0.01f, Color.black);
            //TerrainManager.AddGroundInCube(hit.point, 10);
            TerrainManager.AddGroundInSphere(hit.point, 10);

        }
    }
	
    public override void OnShootAlt()
    {
        Ray ray = new Ray(gunpoint.position, gunpoint.forward); 
        if (Physics.Raycast(ray, out RaycastHit hit, 3000f))
        {
            EffectManager.LineEffect(gunpoint.position, hit.point, 0.01f, Color.green);
            //TerrainManager.RemoveGroundInCube(hit.point, 10);
            TerrainManager.RemoveGroundInSphere(hit.point, 10);
        }
    }

}

