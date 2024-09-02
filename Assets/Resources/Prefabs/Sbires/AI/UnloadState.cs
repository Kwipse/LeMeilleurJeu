using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class UnloadState : AIState 
{
    float _unloadDuration;
   
    MiningAbility _ma;
    

    public override void OnEnter()
    {
              
       // ulong id = sc.gameObject.GetComponent<NetworkObject>().OwnerClientId;
        //GoldSystem gold = PlayerManager.GetPlayerObject(id).GetComponent<RTSPlayer>().gold;

        _ma = sc.gameObject.GetComponent<MiningAbility>();
        _unloadDuration = _ma.unloadDuration;
        _ma.PlayTheAction("Unload");
    }

    public override void OnUpdate()  
    { 
         
        if(_unloadDuration<=0f)
        {
            _ma.GetGold();
           sc.ChangeState(sc.goToMineState);
        }
        else
        {
            _unloadDuration -= sc.deltaTime;
        }

                
   }
 
        public override void OnExit()
        {
          

        }
  
}

