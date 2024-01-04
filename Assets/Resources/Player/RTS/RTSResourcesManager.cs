using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LesMeilleursScripts
{
    public class RTSResourcesManager : MonoBehaviour
    {
        public int gold= 500, goldCap= 2000;
       

        public int GoldAmount()
        {
            return gold;
        }
        public int GoldCapAmount()
        {
            return goldCap;
        }

        public bool Pay(int amount)
        {
            //ceci est un bool, attention a la maniere dont c'est call
            if(gold < amount)
            {
                Debug.Log("trying to pay more than he got");
                return false;
            }
            else
            {
                gold -= amount;
                return true;
            }
        }


    }
}
