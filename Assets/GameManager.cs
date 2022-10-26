using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject CanvasEnd;
    public void FPSWinTrigger()
	{
		Debug.Log("Victoire des FPS");
		CanvasEnd.SetActive(true);
	}

}
