using UnityEngine;
using classes;
using systems;
using scriptablesobjects;
using System;


 public class Cracheur : Unit
{
    public LazerAsset lazerprop;
    public float castDuration= 10.0f,cooldownDuration=10.0f;
    public int castDamage = 20;
    private float castEndTime,cooldownEndTime;

    HealthSystem health;
    string castMode= "ready"; 


    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
        if(Time.time >= castEndTime && castMode == "activated") CastModeExit();    
        if(castMode =="activated") CastModeUpdate();
        if(castMode =="cooldown" && Time.time >= cooldownEndTime) 
            {
                castMode = "ready";
                Debug.Log("cracheur cast ready");
            }  
        }

    /*public override void AttackAction() 
    {
        
    }*/

    public override void SkillAction(Vector3 pos)
    {
            //cast
        Debug.Log("cracheur SKILLACTION");
        if(castMode=="ready") 
        {
            CastModeEnter(pos);
        }
    }

    public void CastModeEnter(Vector3 pos)
    {
        castEndTime = Time.time +castDuration;
        castMode = "activated";
        Debug.Log("cracheur cast start");
        Immobilize();
        Lazer(pos);
    }

    public void CastModeUpdate()
    {}

    public void CastModeExit()
    {
        castMode = "cooldown";        
        Debug.Log("cracheur cast end");        
        cooldownEndTime = Time.time + cooldownDuration;   
        Freed();    
    }

    public void Immobilize()
    {
        MoveOrder(this.gameObject.transform.position,false);
        SetAgentParameterTo(0.0f,0.0f,0.0f);
    }

    public void Freed() 
    {
        SetAgentParameterTo(unitSpeed,unitAngularSpeed,unitAcceleration);
    }

    public void Lazer(Vector3 pos)
    {
         lazerprop.LazerToPos(transform.position, pos);
    }
}

