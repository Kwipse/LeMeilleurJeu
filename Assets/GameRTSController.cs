using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRTSController : MonoBehaviour
{
    private Vector3 startPosition ;
    public GameObject mob;
    
    private void Update()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
           Plane plane = new Plane(Vector3.up, Vector3.zero);

           Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

           float entry;

           if(plane.Raycast(ray,out entry))
           {
               Instantiate(mob,ray.GetPoint(entry), Quaternion.identity)  ;
           }
        }
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.TransformDirection(Vector3.forward), out hit))
            {
                Debug.DrawRay(Camera.main.transform.position,Camera.main.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            }
            
        }
        
    }
}
