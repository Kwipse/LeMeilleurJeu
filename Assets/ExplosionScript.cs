using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag== "Player")
			{collision.collider.GetComponent<HealthSystem>().LoosePv(25);}
	}
}
