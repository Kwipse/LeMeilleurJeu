using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class MiningState :AIState 
    {
        //augmente le stock d or
        //joue l'animation de minage
        //check si on est plein
        MiningAbility _ma;
       [HideInInspector]public float mineTimer=0f;

        public override void OnEnter()
        {
            Debug.Log($"MiningStateEnter");
            _ma = sc.gameObject.GetComponent<MiningAbility>();
            _ma.PlayTheAction();
            mineTimer += _ma.mineCooldown;

        }
        public override void OnUpdate()
        {
            mineTimer -= sc.deltaTime;
 _ma = sc.gameObject.GetComponent<MiningAbility>(); _ma = sc.gameObject.GetComponent<MiningAbility>();          if(mineTimer<=0)
            {
               _ma.GetRessource(); 

                if(_ma.IsFull())  
                {
                    sc.ChangeState("GoToNexusState");
                    
                }

                mineTimer += _ma.mineCooldown;
            }
        }

        public override void OnExit()
        {
            
        }
    }
