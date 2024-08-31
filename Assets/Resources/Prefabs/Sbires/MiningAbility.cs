using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiningAbility : MonoBehaviour
{   
    public List<AIState> _aiStates;
    private Sbire2 _unitHandler;
    //private AIStateController _aisc;
    // specifique
    public GameObject nexus, mine;
    public int loadCapacity = 10, load = 0;
    public float mineCooldown= 1.0f, unloadDuration=2f;
    public int loadByProc=1;

    public float distanceToStop=3f;
    GoldSystem _gold;
    
    public bool isFull=false;

    void Awake()
    {
        //ce truc est dangeureux vvvvvv
        //                       vv  vv 
        //                       v    v
        int count=2;
        foreach(GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name == "Nexus")
            {
                count -=1;
                Debug.Log("miningSbire.cs : nexus found");
                nexus = obj;
            }
            if (obj.name == "GoldMine")
            {
                count -=1;
                Debug.Log("miningSbire.cs : GoldMine found");
                mine = obj;
            }
            if(count==0)
            {
                break;
            }
        }
       // _aisc = gameObject.GetComponent<AIStateController>();
        _unitHandler= gameObject.GetComponent<Sbire2>();
        _gold = _unitHandler.GetOwnerPlayerObject().GetComponent<RTSPlayer>().gold;
        //this.transform.Find("RTSPlayer").

    }

    public void OrdreDeMinage(GameObject _mine)
    {
        if(_mine != null) 
        {
            mine= _mine;  
        }
        if(IsFull())
        {
                
        }
        //sequence
        //si plein , aller vider,vider
        //boucle aller à la mine,miner,aller vider,vider

       
    }

    public void PlayTheAction()
    {
        string actionName= "Mining";
        PlayTheAction(actionName);
       
    }
    public void PlayTheAction(string _name)
    {
 _unitHandler.LaunchAnimationByName(_name);
    }
    public void StopTheAction()
    {
        //communique avec le ai system, lafin de laction

        //arrete l'animation
        _unitHandler.LaunchAnimationByName("Ground Idle");
    }

    public void GetRessource()
    {
        load += loadByProc;
        if(load >= loadCapacity)
        {
            load = loadCapacity;
            isFull=true;
            Debug.Log("load full");
        }
    }

    public bool IsFull()
    {
        if(isFull)
        {
            StopTheAction();
        }
        
        return isFull;
       
    }

    public void MoveOrder(Vector3 position,bool waypointshit = false)
    {
        _unitHandler.MoveOrder(position,waypointshit);
    }


}
