using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GoldSystem : ScriptableObject 
{
    public int InitialGold;
    public int MaxGold;
    [HideInInspector] public Ressource goldRes;

    void Awake()
    {
        goldRes = new Ressource(0, MaxGold, InitialGold);
    }

    //public void SetGold()
    //{
    //    goldRes = new Ressource(0, MaxGold, InitialGold);
    //}

    public void AddGold(int amount) { goldRes.AddRessource(amount); }
    public void RemoveGold(int amount) { goldRes.RemoveRessource(amount); }
    public void SetGold(int amount) { goldRes.SetRessourceTo(amount); }
    public void SetMinGold(int minAmount) { goldRes.SetRessourceMin(minAmount); }
    public void SetMaxGold(int maxAmount) { goldRes.SetRessourceMax(maxAmount); }

    public int GetCurrentGold() { return goldRes.GetRessource(); }
    public bool IsEnoughGold(int testAmount) { return goldRes.TestForRessource(testAmount); }
}
