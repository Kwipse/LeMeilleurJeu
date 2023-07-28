using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSRessourceManager : MonoBehaviour
{
    private static RTSRessourceManager _instance;
    private static RTSRessourceManager Instance { get { return _instance; } }

    public int bankedGold = 500;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void AddGold(int amount)
    {
        bankedGold += amount;
        if (bankedGold < 0) 
        {
            Debug.LogWarning("Bank as a hole(RTSRessourceManager)");
            bankedGold=0; 
        }
    }
    public bool MoreGoldThan(int amount)
    {
        if (bankedGold < amount) { return false; }
        else { return true; }
    }
}
