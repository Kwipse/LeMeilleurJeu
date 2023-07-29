using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FPSPlayerHealth : NetworkBehaviour
{
    public float pv = 100;
    private ulong localId;
    //public bool isAlive = true;
    public void Awake()
    {

        localId = NetworkManager.Singleton.LocalClientId;
    }

    public void LoosePv(int dmg)
    {
        pv -= dmg;
        if (pv <= 0)
        {
            zeroHp();
        }
    }

    public void zeroHp()
    {
        GetComponent<PlayerSpawner>().DestroyPlayer(localId);

        GetComponent<PlayerSpawner>().Spawn("FPSPlayer", Vector3.zero,localId);
    }

    
}
