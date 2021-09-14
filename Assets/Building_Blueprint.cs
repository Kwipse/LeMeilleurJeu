using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Blueprint : MonoBehaviour
{
   
    RaycastHit hit;
    Vector3 movePoint;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);

        if(Physics.Raycast(ray, out hit, 50000.0f, (1<<8)))
        {
            transform.position= hit.point;
            Debug.Log("hitpoint_start: "+ hit.point);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
        }
    }

    // Update is called once per frame
    void Update()
    {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       
        // Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue);

        if(Physics.Raycast(ray, out hit, 50000.0f, (1<<8)))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            transform.position= hit.point;
            Debug.Log("hitpoint: "+ hit.point);

        }
        if(Input.GetMouseButton(0))
        {
            GameObject go = Instantiate(prefab, hit.point, transform.rotation);
            Destroy(gameObject);
        }
    }
}
