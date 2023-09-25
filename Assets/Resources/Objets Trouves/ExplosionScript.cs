using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class ExplosionScript : MonoBehaviour
{
    ulong localId;
    // Start is called before the first frame update
    void Start()
    {
        DestroyCubeServerRpc(0.25f);
    }

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag== "Player")
			{collision.collider.GetComponent<HealthSystem>().LoosePv(25);}
	}

    [ServerRpc(RequireOwnership = false)]
    private void DestroyCubeServerRpc(float dureeOuiNon = 0f)
    {
        //GetComponent<Netw>
        if (dureeOuiNon == 0f)
        {
            Destroy(gameObject);
        }
        else
            Destroy(gameObject, dureeOuiNon);

    }
}
