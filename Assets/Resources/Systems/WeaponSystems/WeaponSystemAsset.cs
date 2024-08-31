using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

[CreateAssetMenu]
public class WeaponSystemAsset : ScriptableObject
{
    //BackPack
    public List<GameObject> AvailableWeapons;
    public List<AmmoSystem> BackpackAmmos;
}
