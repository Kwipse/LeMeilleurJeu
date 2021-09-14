using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;


public class Building_Blueprint : MonoBehaviour
{
    /*
    A attacher à un blueprint de batiment

    le batiment de reste sous le curseur que sur un objet contenant un collider et sur le layer 8
    ce script maintient le blueprint sous la souris et detruit le blueprint pour mettre le batiment correspondant
    */
   
    RaycastHit hit;
    Vector3 movePoint;
    public GameObject prefab;
    public Animator RTSPlayrAnimator;

    
    void Start()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 50000.0f, (1<<8)))
        {
            transform.position= hit.point;
        }
        RTSPlayrAnimator = GameObject.Find("RTSPlayer(Clone)").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 50000.0f, (1<<8)))
        {
            transform.position= hit.point; 

        }
        if(Input.GetMouseButton(0))
        {
            GameObject go = Instantiate(prefab, hit.point, transform.rotation);
            go.GetComponent<NetworkObject>().Spawn();
            if (RTSPlayrAnimator !=null)
            {
                RTSPlayrAnimator.SetBool("IsConstructed", true);
            }
            else
            {
                Debug.Log("RTS Animator not detected");
            }
            Destroy(gameObject);
        }
    }
}
