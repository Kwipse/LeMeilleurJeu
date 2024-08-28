using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAvertizer : MonoBehaviour
{
    public GameObject _bomb;
    Transform gunPoint=null;

    void Awake()
    {
        if(gunPoint == null)
        {
            //SpawnManager.SpawnObject( _bomb, GameObject weapon, Transform altGunpoint, int initialForce = 0);
        }
    }
}
