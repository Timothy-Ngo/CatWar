using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public CircleCollider2D detectionCollider;
    public List<Unit> detectableUnits;
    public Unit targetUnit;
    public bool isSelected
    {
        get => selectionCircle.activeSelf; 
        set => selectionCircle.SetActive(value); 
    }

    [Header("-----ORIENTED PHYSICS-----")]
    public Vector2 position;
    public Vector2 velocity;

    public float heading;
    public float desiredHeading;
    
    public float speed; 
    public float desiredSpeed;

    [Header("-----BATTLE STATS-----")]
    private float atkRange;
    private float detectionRange;
    private float atkTimer;
    public float currentHealth;
    private bool isDead = false;

    [Header("-----RESOURCE COLLECTION-----")]
    public TextMeshProUGUI resourceMoneyText;
    public int resourceDelay = 2;
    public int resourceCost = 10;
    private IEnumerator coroutine;

    public int lootMoney = 10;


    public void Start()
    {
        if (unitType.job == "obstacle")
        {
            position = transform.position;
            return;
        }
        detectableUnits = new List<Unit>();
        unitAI = GetComponent<UnitAI>();
        //selectionCircle = transform.GetChild(0).gameObject;
        Init(transform.position);

        if (unitType.job == "worker")
        {
            resourceMoneyText.text = "+" + resourceCost.ToString();
            resourceMoneyText.gameObject.SetActive(false);
        }
        
    }

    public void Init(Vector2 startingPosition)
    {
        selectionCircle.SetActive(false);
        isSelected = false;
        position = startingPosition;
        transform.position = position;
        //float rad = GetComponent<CircleCollider2D>().radius;
        float rad = detectionCollider.radius;
        atkRange = rad * unitType.atkRangeInRad;
        detectionRange = rad * unitType.detectionRangeInRad;
        currentHealth = unitType.health;
        atkTimer = unitType.atkSpeed;
    }

    void Update()
    {
        if (unitType.job == "nexus" || unitType.job == "obstacle")
        {
            return;
        }
        if (detectableUnits.Count > 0 && targetUnit == null)
        {
            foreach (Unit unit in detectableUnits)
            {
                if (unit == null)
                {
                    detectableUnits.Remove(unit);
                    break;
                }
                //Debug.Log(diff.sqrMagnitude);
                if (unit.unitType.job == "nexus") // if unit is the nexus, then target the nexus
                {
                    targetUnit = unit;
                    break;
                }
                else if ( InDetectionRange(unit) && unitAI.commands.Count == 0) // if the unit is within detection range but out of attack range, then move until within attack range
                {
                    //Debug.Log("In detection ranged");
                    // Math to find out where the nearest point to move to, to be within attack range of the target unit
                    Vector2 diff = unit.position - position;
                    Vector2 normalized = diff.normalized * 1.2f; // 1.2f is so that the point is slightly closer to the target unit within the range so that its not right on the edge 
                    float distToNearbyPoint = diff.magnitude - atkRange;
                    Vector2 nearbyPoint = normalized * distToNearbyPoint ;
                    
                    AIMovement.inst.HandlePriorityMove(this, nearbyPoint + position );
                }
                else if (InAttackRange(unit)) // if the unit the unit is within attack range then attack it
                {
                    //Debug.Log("In Attack ranged");
                    targetUnit = unit;
                    AIMovement.inst.HandlePriorityAttack(this, targetUnit);
                }
                else
                {
                    //Debug.Log("In range");
                }
            }
        }
        else if (targetUnit == null && unitType.job == "player2")
        {
            
        }

        if (targetUnit != null)
        {
            if (atkTimer > 0)
            {
                atkTimer -= Time.deltaTime;
            }
            else
            {
                if (unitType.isRanged) // Ranged Attacks
                {
                    GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    go.gameObject.GetComponent<Projectile>().Init(transform.position,targetUnit.position,  this, targetUnit, unitType.atkDamage);
                    atkTimer = unitType.atkSpeed;
                }
                else // Melee attacks
                {
                    targetUnit.TakeDamage(unitType.atkDamage, this);
                    atkTimer = unitType.atkSpeed;
                }
                Vector2 diff = targetUnit.position - position;
                if (diff.sqrMagnitude > Mathf.Pow(atkRange, 2))
                {
                    targetUnit = null;
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (unitType.job == "obstacle")
        {
            return;
        }
        
        Unit colUnit = col.gameObject.GetComponent<Unit>();
        
        if ( unitType.job == "worker")
        {
            if (col.gameObject.CompareTag("Resources"))
            {
                StartResourceCollection();
                
            }
        }
        if (colUnit == null || colUnit.unitType.job == "obstacle")
        {
            return;
        }
        else
        {
            if ( colUnit != null && colUnit.unitType.faction != unitType.faction)
            {
                detectableUnits.Add(colUnit);
                //Debug.Log($"Added {colUnit} to detectableUnits");
            }
        }
    }

    // Remove unit from detectable units when it leaves the collider 
    public void OnTriggerExit2D(Collider2D col)
    {
        if (unitType.job == "obstacle")
        {
            return;
        }
        Unit colUnit = col.gameObject.GetComponent<Unit>();
        if (colUnit.unitType.job == "obstacle")
        {
            return;
        }
        if (unitType.job == "worker")
        {
            if (col.gameObject.CompareTag("Resources"))
            {
                StopResourceCollection();
            }
        }
        else
        {
            if (colUnit != null && colUnit.unitType.faction != unitType.faction)
            {
                Debug.Assert(detectableUnits.Contains(colUnit));
                detectableUnits.Remove(colUnit);
            }
        }
        
    }

    public bool InDetectionRange(Unit targetUnit)
    {
        Vector2 diff = targetUnit.position - this.position;
        return diff.sqrMagnitude < Mathf.Pow(detectionRange, 2);
    }

    public bool InAttackRange(Unit targetUnit)
    {
        Vector2 diff = targetUnit.position - this.position;
        return diff.sqrMagnitude <= Mathf.Pow(atkRange, 2);
    }
    public void StartResourceCollection()
    {
        resourceMoneyText.gameObject.SetActive(false);
        coroutine = CollectResources();
        StartCoroutine(coroutine);
    }

    // continously collects resources
    IEnumerator CollectResources()
    {

        yield return new WaitForSeconds(resourceDelay);
        resourceMoneyText.gameObject.SetActive(true);
        UI.inst.UpdateCurrencyText(resourceCost);

        UI.inst.ResetTextAlpha(resourceMoneyText);
        UI.inst.FadeTextOut(resourceMoneyText);

        coroutine = CollectResources();
        StartCoroutine(coroutine);

    }

    public void StopResourceCollection()
    {
        resourceMoneyText.gameObject.SetActive(false);
        StopCoroutine(coroutine);
    }
    public void TakeDamage(float dmgAmount, Unit senderUnit)
    {
        if (unitType.job == "obstacle")
        {
            return;
        }
        
        float result = currentHealth - dmgAmount;
        Debug.Log(isDead);
        if (result <= 0 && !isDead) // Kills this unit
        {
            // give player money for killing units
            if (unitType.faction == "player2")
            {
                //UI.inst.UpdateCurrencyText(lootMoney);
            }

            //TODO:make it a method
            
            currentHealth = 0;
            healthBar.localScale = new Vector3(currentHealth / unitType.health, healthBar.localScale.y,
                healthBar.localScale.z);
            Selection.inst.selectedUnits.Remove(this);
            DistanceMgr.inst.RemoveUnit(this); 
            senderUnit.detectableUnits.Remove(this);
            isDead = true;
            Destroy(this.gameObject, 1f);
            Debug.Log($"{gameObject.name} is dead");
        }
        else
        {
            currentHealth = result;
            healthBar.localScale = new Vector3(currentHealth / unitType.health, healthBar.localScale.y,
                healthBar.localScale.z);
        }
    }
}
