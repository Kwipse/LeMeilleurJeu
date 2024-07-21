using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TowerScript : MonoBehaviour
{
    /*
     * etat idle 
     * etat attaque
     * type de projectile
     * cd
     * aleatoire angle et force
     */
    ulong localId;
    public GameObject bomb;
    public float attaqueRate= 1.0f;
    public float angleDelta, forceDelta;
    private float nextAttackTick = 0.0f;

    
    // Start is called before the first frame update
    void Start()
    {
        localId = NetworkManager.Singleton.LocalClientId;
    }

    // Update is called once per frame
    void Update()
    {
        if(nextAttackTick < Time.time) 
        {
            nextAttackTick = Time.time+attaqueRate;
            //SM.Spawn("Bomb", transform.Find("GunPoint").position, localId, transform.Find("GunPoint").rotation);
            SpawnManager.SpawnObject("Bomb", new Vector3(0,30,40), Quaternion.identity);
        }

    }
}
