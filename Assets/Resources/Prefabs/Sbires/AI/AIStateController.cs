using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateController : MonoBehaviour
{
    int procCounter=0,procRate=12;

    AIState currentState;

    public MiningState miningState = new MiningState();

    public SleepState sleepState = new SleepState();
    public HurtState hurtState = new HurtState();

    public float deltaTime=0f;

    private void Start()
    {
        ChangeState(sleepState);
    }

    void Update()
    {  
        if (currentState != null)
        {
            if(procCounter == procRate)
            {
                //les states utilisent deltaTime pour les durées d'animation
                procCounter = 0;

                currentState.OnUpdate();

            }
            else
            {
                procCounter += 1;
            }
        }

    }

    public void ChangeState(AIState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter();
    }

}

