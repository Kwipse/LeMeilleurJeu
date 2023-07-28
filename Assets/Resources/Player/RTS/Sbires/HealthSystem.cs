using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    public float pv=100;
	public bool isNexus;

    public void LoosePv(int dmg)
    {
		Debug.Log("hit");
        pv -= dmg;
		if(pv== 0)
        {
            zeroHp();
        }
    }
	
	public void zeroHp()
	{
		if(isNexus)
		{
			GameObject.Find("GameManager").GetComponent<GameManager>().FPSWinTrigger();
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
