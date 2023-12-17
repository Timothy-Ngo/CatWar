using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    
    
    private AIState currentState;
    public AIInitialState initialState;
    public int cash;
    
    // Start is called before the first frame update
    void Awake()
    {
        ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.StateUpdate();
    }

    public void ChangeState(AIState newState)
    {
        currentState = newState;
    }
    
    
}
