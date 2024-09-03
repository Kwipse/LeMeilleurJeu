using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MiningAbility))]
[RequireComponent(typeof(AIStateController))]
public class Sbire2 : Unit
{
// communique avec les navemeshagent, l'annimator et le systeme IA et l'ordre buffer
// recoit des ordres , renvoi la fin de l'ordre
//
    HealthSystem health;
    public AIStateController _aisc; 
    public MiningAbility _ma;
    public Animator _anim;
    OrderBuffer _ob;

    public override void Awake()
    {

        _ob=GetComponent<OrderBuffer>();
        health = GetComponent<HealthSystem>();
        if( _aisc == null)
        {
            Debug.Log($"heu___yapasd'ia");
        }
        _ma = GetComponent<MiningAbility>();
        _anim = gameObject.GetComponent<Animator>();
        base.Awake();
       

    }

    public void LaunchOrder(OrderRTS ordre)
    {
        if(ordre.name == "MoveOrder")
        {
            //MoveAction(ordre.pos); //NdN : Ca lance des warnings/erreurs
        }
         if(ordre.name == "SkillOrder")
         {
             //ceci est a changé
             Debug.Log("sbire2.ActionOrder Launching order");
             //SkillAction(Vector3.zero); //NdN : Ca lance des warnings/erreurs

         }
    }
    public override void AttackAction() 
    {}

    public void LaunchAnimationByName(string _name)
    {
        _anim.Play(_name);
    }
 

    public override void MoveAction(Vector3 pos, bool addWaypoint = false)
    {
        //permet de lancer un ordre de déplacement depuis la souris
        _ma.StopOrdre();
        base.MoveAction(pos,addWaypoint);
    }

    public void MoveActionInAISequence(Vector3 pos, bool addWaypoint = false)
    {
        //lance un déplacement pendant une sequence d'ia
        base.MoveActionInSequence(pos,addWaypoint); 
    }
    public override void SkillAction(Vector3 pos)
    {
        _ma.OrdreDeMinage();
    }

    public override List<OrderRTS> GetAvailableOrder()
    {
        //remplit le order buffer avec les ordres valables
        List<OrderRTS> avOrder=new List<OrderRTS>();
       // avOrder.Add((OrderRTS)MoveOrder);
         //avOrder.Add((OrderRTS)SkillOrder);
        //renvoi les ordres des differents ability
        //move order
        return avOrder;
         

    }
} 
