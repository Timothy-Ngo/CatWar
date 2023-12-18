using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawner : MonoBehaviour
{
    public Units units; 
    public GameObject unitPrefab;
    public GameObject rallyPointObject;
    [Header("-----PLAYER-----")] 
    public float buyInterval = 3f;
    private float buyTimer = 0;
    public Image buttonImage; // Needs to be dragged and dropped
    public RectTransform buyCooldownBar; // Needs to be dragged and dropped
    
    
    [Header("-----AI-----")] 
    public bool isAI;
    
    // Rally points near player1 nexus
    public GameObject attackRallyPointsParent;
    public List<Vector2> attackRallyPoints;
    // Rally points near player2 nexus
    public GameObject defenceRallyPointsParent;
    public List<Vector2> defenceRallyPoints;
    
    // Rally points near the player2 side of the fish farms
    public GameObject farmRallyPointsParent;
    public List<Vector2> farmRallyPoints;
    
    public float spawnInterval = 3f;
    private float spawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        if (isAI)
        {
            spawnTimer = spawnInterval;
            foreach (Transform child in attackRallyPointsParent.transform)
            {
                attackRallyPoints.Add(child.position);
            }
            foreach (Transform child in defenceRallyPointsParent.transform)
            {
                defenceRallyPoints.Add(child.position);
            }
            foreach (Transform child in farmRallyPointsParent.transform)
            {
                farmRallyPoints.Add(child.position);
            }
        }
        else
        {
            buyTimer = 0;
            buyCooldownBar.localScale = new Vector3(buyTimer / buyInterval, buyCooldownBar.localScale.y,
                buyCooldownBar.localScale.z);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAI)
        {
            return;
            if (spawnTimer > 0)
            {
                spawnTimer -= Time.deltaTime;
            }
            else
            {
                RandomSpawn(1, unitPrefab, transform.position, attackRallyPoints);
                spawnTimer = spawnInterval;
            }
        }
        else
        {
            
            if (buyTimer > 0)
            {
                buyTimer -= Time.deltaTime;
                buyCooldownBar.localScale = new Vector3(buyTimer / buyInterval, buyCooldownBar.localScale.y,
                    buyCooldownBar.localScale.z);
            }
            else
            {
                buttonImage.color = Color.white;
            }
        }
    }

    public void Spawn(int num, GameObject unitPrefab, Vector2 spawnPos, Vector2 rallyPoint)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
            Unit newUnit = go.GetComponent<Unit>();
            newUnit.Init(spawnPos);
            units.Add(newUnit);
            newUnit.position = spawnPos;
            DistanceMgr.inst.Initialize();
            AIMovement.inst.HandleSetOneMove(newUnit, rallyPoint);
        }
    }

    public void RandomSpawn(int num, GameObject unitPrefab, Vector2 spawnPos, List<Vector2> rallyPoints) // Make a unit move to a random rally point 
    {
        int randomIndex = Random.Range(0, rallyPoints.Count);
        Vector2 point = rallyPoints[randomIndex];
        Spawn(num, unitPrefab, spawnPos, point);
    }
    
    
    public void SpawnUnit(GameObject unitPrefab)
    {
        if (UI.inst.GetCurrency() >= unitPrefab.GetComponent<Unit>().unitType.cost)
        {
            Debug.Assert(!isAI, "AI called this function, this shouldn't happen");
            if (buyTimer <= 0)
            {
                Vector2 rallyPoint = rallyPointObject.transform.position;
                Spawn(1, unitPrefab, transform.position, rallyPoint);
                buyTimer = buyInterval;
                Economy.inst.BuyCat(unitPrefab.GetComponent<Unit>().unitType);
            
                // Change color
                buttonImage.color = Color.gray;
            }
        }
    }
    
    
}
