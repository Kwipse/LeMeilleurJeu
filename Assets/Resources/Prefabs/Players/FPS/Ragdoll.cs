using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    void Awake()
    {
        ColorManager.SetObjectColors(gameObject);
        Invoke("SelfDestroy", 60);
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
