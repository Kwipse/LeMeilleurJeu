using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float pv=100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pv== 0)
        {
            Destroy(gameObject);
        }

    }

    public void LoosePv(int dmg)
    {
		Debug.Log("hit");
        pv -= dmg;
    }
}
