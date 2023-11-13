using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HouseScript : MonoBehaviour
{
    void Start()
	{
		transform.Find("RTSManager").GetComponent<RTSRessourceManager>().AddPopCap(10);
	}
}
