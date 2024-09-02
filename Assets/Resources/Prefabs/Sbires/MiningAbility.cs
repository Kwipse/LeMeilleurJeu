using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiningAbility : SyncedBehaviour, IWaitForGameSync
{   
    public List<AIState> _aiStates;
    private Sbire2 _unitHandler;
    private AIStateController _aisc;
    // specifique
    public GameObject nexus, mine;
    private bool noMine=true;
    public int loadCapacity = 10, load = 0;
    public float mineCooldown= 1.0f, unloadDuration=2f;
    public int loadByProc=1;

    public float distanceToStop=3f;
    GoldSystem _gold;

    public bool isFull=false;

    public override void StartAfterGameSync()
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
                noMine=false;
            }
            if(count==0)
            {
                break;
            }
        }
        _aisc = gameObject.GetComponent<AIStateController>();
        _unitHandler= gameObject.GetComponent<Sbire2>();
        _gold = _unitHandler.GetOwnerPlayerObject().GetComponent<RTSPlayer>().gold;
        //this.transform.Find("RTSPlayer").

    }

    public void OrdreDeMinage(GameObject _mine = null)
    {

        if(_mine != null) 
        {
            mine= _mine;  
            noMine = false;
        }
        else 
        {
            if(noMine)
            {
                StopOrdre();
                return ;
            }

        }

        if(IsFull())
        {
            _aisc.ChangeState(_aisc.goToNexusState);
        }
        _aisc.ChangeState(_aisc.goToMineState);

    }

    public void StopOrdre()
    {
        _aisc.ChangeState(_aisc.sleepState); 
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
        //remplit le sac de l'unit
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

    public void MoveAction(Vector3 position,bool waypointshit = false)
    {

        _unitHandler.MoveAction(position,waypointshit);
    }

    public void MoveActionInSequence(Vector3 position,bool waypointshit = false)
    {

        _unitHandler.MoveActionInSequence(position,waypointshit);
    }



    public void GetGold()
    {
        //fortheRTS
        _gold.AddGold(load);
        //empty le load
        load = 0;
        isFull = false;

    }
}

