using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public Units units; 
    public GameObject unitPrefab;
    public GameObject rallyPointObject;

    [Header("-----AI-----")] 
    public bool isAI;

    public float spawnInterval = 3f;

    private float spawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {

        if (isAI)
        {
            if (spawnTimer > 0)
            {
                spawnTimer -= Time.deltaTime;
            }
            else
            {
                Vector2 rallyPoint = rallyPointObject.transform.position;
                Spawn(1, unitPrefab, transform.position, rallyPoint);
                spawnTimer = spawnInterval;
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

    public void SpawnUnit(GameObject unitPrefab)
    {
        Vector2 rallyPoint = rallyPointObject.transform.position;
        Spawn(1, unitPrefab, transform.position, rallyPoint);
    }
    
    
}
