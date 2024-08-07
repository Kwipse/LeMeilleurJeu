using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AmmoSystem : ScriptableObject
{
    public string ammoType;
    public int maxAmmo;
    Ressource ammoRes;

    public AmmoSystem(string AmmoType, int MaxAmmo)
    {
        ammoType = AmmoType;
        maxAmmo = MaxAmmo;
    }

    public void OnEnable()
    {
        ammoRes = new Ressource(0, maxAmmo, maxAmmo);
    }

    public void SetMaxAmmo(int newMax) { UpdatemaxAmmo(newMax); }

    public void AddAmmo(int amount) { ammoRes.AddRessource(amount); }
    public void RemoveAmmo(int amount) { ammoRes.RemoveRessource(amount); }
    public void SetAmmoToFull() { ammoRes.SetRessourceToMax(); }
    public void SetAmmoToEmpty() { ammoRes.SetRessourceToMin(); }

    public int GetAmmo() { return ammoRes.GetRessource(); }
    public Ressource GetAmmoRessource() { return ammoRes; }
    public int GetMaxAmmo() { return ammoRes.GetRessourceMax(); }
    public string GetAmmoType() { return ammoType; }
    public bool IsEnoughAmmo(int testAmount) { return ammoRes.TestForRessource(testAmount); }
    public bool IsAmmoFull() { return ammoRes.TestForRessource(maxAmmo); }

    void UpdatemaxAmmo(int newMax) {
        maxAmmo = newMax;
        ammoRes.SetRessourceMax(maxAmmo); }

}
