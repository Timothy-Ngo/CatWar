using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public Units units; 
    public GameObject unitPrefab;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 rallyPoint = (Vector2)transform.position  + new Vector2(10, 0);
            Spawn(1, unitPrefab, transform.position, rallyPoint);
        }

        if (isAI)
        {
            if (spawnTimer > 0)
            {
                spawnTimer -= Time.deltaTime;
            }
            else
            {
                Vector2 rallyPoint = (Vector2)transform.position + new Vector2(-50, 9);
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
            Move m = new Move(newUnit, rallyPoint);
            UnitAI uai = newUnit.GetComponent<UnitAI>();
            Debug.Assert(uai != null);
            uai.SetCommand(m);
        }
    }

    public void SpawnUnit(GameObject unitPrefab)
    {
        Vector2 rallyPoint = (Vector2)transform.position  + new Vector2(10, 0);
        Spawn(1, unitPrefab, transform.position, rallyPoint);
    }
    
    
}
