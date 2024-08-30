using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiningAbility : MonoBehaviour
{   
    public GameObject nexus, mine;

    public int loadCapacity = 10, load = 0;
    public float mineCooldown= 1.0f;
    public int loadByProc=1;
    public GoldSystem _gold;
    private Sbire2 _unitHandler;
    public bool isFull=false;

    void Awake()
    {
        //ce truc est dangeureux vvvvvv
        //                       vv  vv 
        //                       v    v
        foreach(GameObject obj in FindObjectsOfType<GameObject>())
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
        }
        _unitHandler= gameObject.GetComponent<Sbire2>();
        _gold = _unitHandler.GetOwnerPlayerObject().GetComponent<RTSPlayer>().gold;
            //this.transform.Find("RTSPlayer").

    }
   public void PlayTheAction()
   {
        string actionName= "Mining";
        _unitHandler.LaunchAnimationByName(actionName);


   }

   public void GetRessource()
   {
        load += loadByProc;
        
   
       if(load >= loadCapacity)
        {
            load = loadCapacity;
            isFull=true;
        }
   }
       
   public bool IsFull()
   {
       return isFull;
   }

  
}
