using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Unit Gameobject script
/// </summary>
[System.Serializable]
public class Unit : MonoBehaviour
{
    public UnitType unitType;
    private GameObject selectionCircle;
    public List<Unit> detectableUnits;
    
    public bool isSelected
    {
        set { selectionCircle.SetActive(value); }
        get { return selectionCircle.activeSelf; }
    }
    public Vector2 position;
    public Vector2 velocity;

    public float heading;
    public float desiredHeading;
    
    public float speed; 
    public float desiredSpeed;
    

    public void Start()
    {
        detectableUnits = new List<Unit>();
        selectionCircle = transform.GetChild(0).gameObject;
        Init(transform.position);
    }

    public void Init(Vector2 startingPosition)
    {
        selectionCircle.SetActive(false);
        isSelected = false;
        position = startingPosition;
        transform.position = position;
        float rad = GetComponent<CircleCollider2D>().radius;
        unitType.detectionRadius = rad * 0.75f;
        unitType.atkRange = rad * 0.5f;
    }

    void Update()
    {
        if (detectableUnits.Count > 0)
        {
            foreach (Unit unit in detectableUnits)
            {
                Vector2 diff = unit.position - position;
                //Debug.Log(diff.sqrMagnitude);
                if (diff.sqrMagnitude < Mathf.Pow(unitType.atkRange, 2))
                {
                    
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        Unit colUnit = col.gameObject.GetComponent<Unit>();
        if ( colUnit != null && colUnit.unitType.faction != unitType.faction)
        {
            detectableUnits.Add(colUnit);
            Debug.Log($"Added {colUnit} to detectableUnits");
        }
    }

    // Remove unit from detectable units when it leaves the collider 
    public void OnTriggerExit2D(Collider2D col)
    {
        Unit colUnit = col.gameObject.GetComponent<Unit>();
        Debug.Assert(detectableUnits.Contains(colUnit));
        detectableUnits.Remove(colUnit);
    }
}
