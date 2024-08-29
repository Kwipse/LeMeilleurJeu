using UnityEngine;


public class Flingue : Arme
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
            Debug.Log($"Hit {hit.transform.gameObject.name} at {hit.point}");
            EffectManager.SpawnTrace("Trace", hit);
        }
    }
	
    public override void OnShootAlt()
    {

    }

}

