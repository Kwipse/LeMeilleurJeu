using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject cv;
    public void FPSWinTrigger()
	{
		Debug.Log("Victoire des FPS");
		cv.SetActive(true);
	}

}
