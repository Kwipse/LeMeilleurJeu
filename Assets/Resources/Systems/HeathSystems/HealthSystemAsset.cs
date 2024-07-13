using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


[CreateAssetMenu]
public class HealthSystemAsset : ScriptableObject
{
    public int hpPool = 100;
    public HealthBar lifeBar;
}

