using UnityEngine;
using System;

public class Mammouth : Unit
{
    public float chargeSpeed = 100.0f,chargeAngularSpeed , chargeAcceleration= 300.0f;
    public float chargeDuration= 10.0f,cooldownDuration=10.0f;
    public int chargeDamage = 20;
    private float chargeEndTime,cooldownEndTime;

    HealthSystem health;
    string ChargeMode= "ready"; 

    public override void Awake()
    {
        //health = GetComponent<HealthSystem>();
        chargeAngularSpeed= unitAngularSpeed;
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
        if(Time.time >= chargeEndTime && ChargeMode == "activated") ChargeModeExit();    
        if(ChargeMode =="activated") ChargeModeUpdate();
        if(ChargeMode =="cooldown" && Time.time >= cooldownEndTime) 
            {
                ChargeMode = "ready";
                Debug.Log("Mammouth charge ready");
            }  
    }

    /*public override void AttackAction() 
    {
       
    }*/

    public override void SkillAction(Vector3 pos)
    {
        //charge
    
        
        if(ChargeMode=="ready") 
            {
                ChargeModeEnter();
                MoveAction(pos,false);
            }

    }

    private void ChargeModeEnter()
    {
        chargeEndTime = Time.time +chargeDuration;
        ChargeMode = "activated";
        Debug.Log("Mammouth charge start");
        SetAgentParameterTo(chargeSpeed,chargeAngularSpeed,chargeAcceleration);
    }
    private void ChargeModeUpdate()
    {}

    private void ChargeModeExit()
    {
        ChargeMode = "cooldown";
        Debug.Log("Mammouth charge end");
        cooldownEndTime = Time.time + cooldownDuration;
        SetAgentParameterTo(unitSpeed,unitAngularSpeed,unitAcceleration);
    }



    void OnCollisionEnter(Collision other)
    {
        if(ChargeMode =="activated")
        {
            GameObject target= other.gameObject;
            target.GetComponent<HealthSystem>()?.LoosePv(chargeDamage);
        }
    }
}
