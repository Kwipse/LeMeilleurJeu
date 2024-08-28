using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistOfFury : Arme
{
    public GameObject FofPolygone;
      
    public override void OnShoot()
    {
        Debug.Log("fist of fury" );
        // instantiate
        //GameObject _fofPolygone = Instantiate(FofPolygone,transform.position,Quaternion.identity);
        gameObject.GetComponent<Animation>().Play();

    }
}

