using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Unit nexus;
    public GameObject unitsParent;
    bool changeToAttack = true;

    private AIState currentState;
    public AIInitialState initialState;
    public AIHalfDefenseState halfDefenseState;
    public AIFullDefenseState fullDefenseState;
    public AIAttackState attackState;
    public int cash;
    
    // Start is called before the first frame update
    void Awake()
    {
        ChangeState(initialState);
    }
    float nexusHealth;
    // Update is called once per frame
    void Update()
    {
        currentState.StateUpdate();

        
        if (currentState == fullDefenseState)
        {
            
            changeToAttack = true;
            List<Unit> units = new List<Unit>();
            // get the ai's current units
            foreach (Transform unit in unitsParent.transform)
            {
                units.Add(unit.gameObject.GetComponent<Unit>());
            }

            if (units.Count > 7)
            {
                Debug.Log("changed to attack state");
                ChangeState(attackState);
            }

            /*
            // if the units have no target unit then change to attack state
            foreach (Unit unit in units)
            {
                Debug.Log(unit.name);
                Debug.Log(unit.targetUnit.name);
                if (unit.targetUnit != null)
                {
                    changeToAttack = false;
                }
            }

             */
            
        }
        else if (nexus.currentHealth <= 100f && currentState != fullDefenseState && currentState != attackState)
        {
            Debug.Log("changed to full defense state");
            ChangeState(fullDefenseState);
        }
        else if (nexus.currentHealth < 150f && currentState == initialState)
        {
            Debug.Log("changed to half defense state");
            ChangeState(halfDefenseState);
        }
        else if (currentState == fullDefenseState && changeToAttack)
        {
            changeToAttack = false;
            Debug.Log("changed to attack state");
            ChangeState(attackState);
        }
        else if (currentState == attackState)
        {
            List<Unit> units = new List<Unit>();
            // get the ai's current units
            foreach (Transform unit in unitsParent.transform)
            {
                units.Add(unit.gameObject.GetComponent<Unit>());
            }

            if (units.Count < 5)
            {
                Debug.Log("changed to full defense state");
                ChangeState(fullDefenseState);
            }
        }
        //else if (nexus.currentHealth == nexusHealth && currentState != initialState)
        //{
        //    ChangeState(initialState);
        //}

        //nexusHealth = nexus.currentHealth;
    }

    public void ChangeState(AIState newState)
    {
        //Debug.Log("changed state");
        currentState = newState;
    }
    
    
}
