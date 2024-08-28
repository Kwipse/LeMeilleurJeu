using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trace : MonoBehaviour
{

    void Awake()
    {
        Invoke("DestroyTrace",10);
    }

    void DestroyTrace()
    {
        Destroy(gameObject);
    }
}
