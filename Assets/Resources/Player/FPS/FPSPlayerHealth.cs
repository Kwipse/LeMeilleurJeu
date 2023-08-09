using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FPSPlayerHealth : MonoBehaviour
{
    
	SpawnManager SM;
	
	public float pv = 100;
    private ulong localId;
    //public bool isAlive = true;
	
	
	
    void Awake()
    {
		SM = GetComponent<SpawnManager>();
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
        SM.DestroyPlayer(localId);

        SM.SpawnPlayer("FPSPlayer", Vector3.zero);
    }

    
}
