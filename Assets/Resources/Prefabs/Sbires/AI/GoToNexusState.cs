using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GoToNexusState : AIState 
{
    float distanceToStop=0f;
    MiningAbility _ma;
    [HideInInspector] public bool isArrived=false;
    public override void OnEnter()
    {
        _ma = sc.gameObject.GetComponent<MiningAbility>();
        distanceToStop= _ma.distanceToStop;
        _ma.MoveOrder(_ma.nexus.transform.position, false);
    }

    public override void OnUpdate()  
    {
       Vector3 gap = _ma.nexus.transform.position - sc.transform.position;
        float distanceGap = gap.sqrMagnitude;
        if(distanceToStop*distanceToStop>distanceGap)
        {
            sc.ChangeState("UnloadState");
        }
                
   }
  
}

