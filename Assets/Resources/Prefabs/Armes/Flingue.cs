using UnityEngine;
using classes;

public class Flingue : Arme
{	
    Transform gunpoint;


    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        gunpoint = GetGunpointTransform();
        base.Start(); 
    }
	
	public override void OnShoot()
    {
        Ray ray = new Ray(gunpoint.position, gunpoint.forward); 
        if (Physics.Raycast(ray, out RaycastHit hit, 3000f))
        {
            Debug.Log($"Hit {hit.transform.gameObject.name} at {hit.point}");
        }
    }
	
    public override void OnShootAlt()
    {

    }

}

