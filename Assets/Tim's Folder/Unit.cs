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
    public UnitAI unitAI;
    public GameObject projectilePrefab;
    public RectTransform healthBar;
    public UnitType unitType;
    public GameObject selectionCircle;
    public List<Unit> detectableUnits;
    public Unit targetUnit;
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
    private float atkRange;
    private float detectionRange;
    private float atkTimer;
    public float currentHealth;
    
    public void Start()
    {
        detectableUnits = new List<Unit>();
        unitAI = GetComponent<UnitAI>();
        //selectionCircle = transform.GetChild(0).gameObject;
        Init(transform.position);
    }

    public void Init(Vector2 startingPosition)
    {
        selectionCircle.SetActive(false);
        isSelected = false;
        position = startingPosition;
        transform.position = position;
        float rad = GetComponent<CircleCollider2D>().radius;
        atkRange = rad * unitType.atkRangeInRad;
        detectionRange = rad * unitType.detectionRangeInRad;
        currentHealth = unitType.health;
        atkTimer = unitType.atkSpeed;
    }

    void Update()
    {
        if (detectableUnits.Count > 0 && targetUnit == null)
        {
            foreach (Unit unit in detectableUnits)
            {
                if (unit == null)
                {
                    detectableUnits.Remove(unit);
                    continue;
                }
                Vector2 diff = unit.position - position;
                //Debug.Log(diff.sqrMagnitude);
                if (diff.sqrMagnitude <= Mathf.Pow(atkRange, 2))
                {
                    //Debug.Log("In Attack ranged");
                    targetUnit = unit;
                }
                else if (diff.sqrMagnitude < Mathf.Pow(detectionRange, 2) && unitAI.commands.Count == 0)
                {
                    //Debug.Log("In deterction ranged");
                    Vector2 normalized = diff.normalized;
                    float distToNearbyPoint = diff.magnitude - atkRange;
                    Vector2 nearbyPoint = normalized * distToNearbyPoint * 1.5f;
                    List<Unit> unitList = new List<Unit>();
                    unitList.Add(this);
                    AIMovement.inst.HandleMove(unitList, nearbyPoint + position );
                }
                else
                {
                    //Debug.Log("In range");
                }
            }
        }

        if (targetUnit != null)
        {
            if (atkTimer > 0)
            {
                atkTimer -= Time.deltaTime;
            }
            else
            {
                if (unitType.faction == "player2")
                    Debug.Log("player 2 unit");
                if (unitType.isRanged)
                {
                    GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    go.gameObject.GetComponent<Projectile>().Init(transform.position,targetUnit.position,  this, targetUnit, unitType.atkDamage);
                    atkTimer = unitType.atkSpeed;
                }
                else
                {
                    Debug.Log($"Melee attacking {targetUnit}");
                }
                Vector2 diff = targetUnit.position - position;
                if (diff.sqrMagnitude > Mathf.Pow(atkRange, 2))
                {
                    targetUnit = null;
                }
            }
        }
        else
        {
            
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        Unit colUnit = col.gameObject.GetComponent<Unit>();
        if ( colUnit != null && colUnit.unitType.faction != unitType.faction)
        {
            detectableUnits.Add(colUnit);
            //Debug.Log($"Added {colUnit} to detectableUnits");
        }
    }

    // Remove unit from detectable units when it leaves the collider 
    public void OnTriggerExit2D(Collider2D col)
    {
        Unit colUnit = col.gameObject.GetComponent<Unit>();
        if (colUnit != null && colUnit.unitType.faction != unitType.faction)
        {  
            Debug.Assert(detectableUnits.Contains(colUnit));
            detectableUnits.Remove(colUnit);
        }
    }

    public void TakeDamage(float dmgAmount, Unit senderUnit)
    {
        float result = currentHealth - dmgAmount;
        if (result <= 0)
        {
            //TODO: Do something  here to kill Unit, make it a method
            currentHealth = 0;
            healthBar.localScale = new Vector3(currentHealth / unitType.health, healthBar.localScale.y,
                healthBar.localScale.z);
            Selection.inst.selectedUnits.Remove(this);
            DistanceMgr.inst.potentialsDictionary.Remove(this);
            DistanceMgr.inst.playerUnits.units.Remove(this);
            DistanceMgr.inst.otherPlayerUnits.units.Remove(this);
            senderUnit.detectableUnits.Remove(this);
            Destroy(this.gameObject, 1f);
        }
        else
        {
            currentHealth = result;
            healthBar.localScale = new Vector3(currentHealth / unitType.health, healthBar.localScale.y,
                healthBar.localScale.z);
        }
    }
}
