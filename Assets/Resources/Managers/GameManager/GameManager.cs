using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
	public GameObject CanvasEnd;
    public void FPSWinTrigger()
	{
		Debug.Log("Victoire des FPS");
		CanvasEnd.SetActive(true);
	}



}
