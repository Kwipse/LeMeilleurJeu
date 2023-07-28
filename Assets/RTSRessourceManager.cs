using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSRessourceManager : MonoBehaviour
{
    private static RTSRessourceManager _instance;
    private static RTSRessourceManager Instance { get { return _instance; } }

    public int 	bankedGold = 500,
				goldCap = 2000,
				actualPop = 1,
				popCap = 20,
				storedEnergy = 0,
				energyCap = 100;


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
		actualPop = GetComponent<UnitList>().
    }

    public void AddGold(int amount)
    {
        bankedGold += amount;
        if (bankedGold < 0) 
        {
            Debug.LogWarning("Bank as a hole(RTSRessourceManager)");
            bankedGold=0; 
        }
		if(bankedGold > goldCap)
		{
			bankedGold = goldCap ;
		}
    }
    public bool MoreGoldThan(int amount)
    {
        if (bankedGold < amount) { return false; }
        else { return true; }
    }
	public void AddGoldCap(int amount)
	{
		goldCap+=amount;
		if (goldCap<0) goldCap = 0;
	}
	
	public bool isPopCapPassedWith(int amount=1)
	{
		if(actualPop+amount > popCap)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public void UnitCreationCount(int amount = 1)
	{
		actualPop += amount ;
	}
	public void UnitDeathCount(int amount = 1)
	{
		actualPop -= amount ;
	}
	public void AddPopCap(int amount)
	{
		popCap+=amount;
		if (popCap<0) popCap = 0;
	}
	public void RefreshActualPop()
	{
		//a faire
	}
	
	public void AddEnergy(int amount)
	{
		storedEnergy+=amount;
		if (storedEnergy<0) storedEnergy = 0;
		if (storedEnergy>energyCap) storedEnergy = energyCap;
	}
	public void AddEnergyCap(int amount)
	{
		energyCap+=amount;
		if (energyCap<0) energyCap = 0;
	}
	 public bool MoreEnergyThan(int amount)
    {
        if (storedEnergy < amount) { return false; }
        else { return true; }
    }
	
}
