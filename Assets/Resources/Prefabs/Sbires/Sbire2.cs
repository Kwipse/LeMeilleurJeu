using UnityEngine;
using classes;
using systems;
using scriptablesobjects;

namespace LesMeilleursScripts
{
    public class Sbire2 : Unit
    {
        //on experimente le minage
        HealthSystem health;
        public GameObject nexus, mine;
        public RTSRessourceManager _rM;
        public int loadCapacity = 10, load = 0;

        public GoldSystem Gold;

    public override void Awake()
    {
        health = GetComponent<HealthSystem>();
        base.Awake();foreach(GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name == "Nexus")
            {
                Debug.Log("miningSbire.cs : nexus found");
                nexus = obj;
            }
            if (obj.name == "GoldMine")
            {
                Debug.Log("miningSbire.cs : GoldMine found");
                mine = obj;
            }
            if (obj.name == "RTSManager")
            {
                Debug.Log("miningSbire.cs : RtsManager found");
               _rM = obj.GetComponent<RTSRessourceManager>();
            }
        }
        
    } 


    public override void AttackAction() 
    {}

    }
}
