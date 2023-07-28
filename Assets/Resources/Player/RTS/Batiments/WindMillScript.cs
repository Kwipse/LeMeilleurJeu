using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	public float tickRate = 5.0f, nextTick = 0.0f;
	
	public void Update()
	{
		if(nextTick<= Time.time)
		{
			nextTick = Time.time + tickRate;
			transform.Find("RTSManager").GetComponent<RTSManager>().AddEnergy(10);

		}
	}
}
