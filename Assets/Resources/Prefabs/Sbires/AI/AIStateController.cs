using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class AIStateController : MonoBehaviour
{
    int procCounter=0,procRate=12;

    public  AIState currentState;
    [SerializeField]
    public string currentStateName;

    public MiningState miningState = new MiningState();
    public GoToNexusState goToNexusState = new GoToNexusState();
    public GoToMineState goToMineState = new GoToMineState();
    public UnloadState unloadState = new UnloadState();

    public SleepState sleepState = new SleepState();
    public HurtState hurtState = new HurtState();

    // this is for aiState which all have lower rate than update
    [HideInInspector]public float deltaTime=0f;
    //private float stackedDelta=0f;

    private void Start()
    {
        Debug.Log($"Bonjour");
        ChangeState(sleepState);
        currentStateName= currentState.name;
    }

    void Update()
    {  
        if (currentState != null)
        {
            if(procCounter == procRate)
            {
                

                //les states utilisent deltaTime pour les durées d'animation
                procCounter = 0;

                currentState.OnStateUpdate();
                ResetDeltaTime();
            }
            else
            {
                StackDeltaTime();
                procCounter += 1;
            }
        }

    }

    public void ChangeState(AIState newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = newState;
        currentState.OnStateEnter(this);
    }

    public void ChangeState(string _name)
    {

    }

    private void StackDeltaTime()
    { 
       
        deltaTime += Time.deltaTime;   
    }

    private void ResetDeltaTime()
    {
        deltaTime = 0f;
    }

}

