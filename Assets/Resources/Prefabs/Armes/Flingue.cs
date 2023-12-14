using UnityEngine;
using classes;

public class Flingue : Arme
{	
    public override void Start()
    {
        base.Start(); //Appel de la fonction Start() de la classe mere
    }
	
	public override void OnShoot()
    {
        Debug.Log("Shoot !");
    }
	
    public override void OnShootAlt()
    {
        Debug.Log("ShootAlt !");
    }

}

