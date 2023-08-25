using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FPSPlayerHealth : MonoBehaviour
{
    
	
	public float pv = 100;
    private ulong localId;
    //public bool isAlive = true;
    SpawnManager SM;
	
	
    void Awake()
    {
        SM = (SpawnManager) SpawnManager.spawner;
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
        SM.DestroyPlayer();

        SM.SpawnPlayer("FPSPlayer", Vector3.zero);
    }

    
}
