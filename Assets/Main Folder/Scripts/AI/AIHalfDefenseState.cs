using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHalfDefenseState : AIState
{
    // starts sending spawned cats to defend the nexus
    // any current cats out will go to the farm points to take out worker cats


    // current units
    public GameObject unitsParent;

    // AI Data
    protected int initialCash;

    // Spawning Data
    public float spawnInterval = 3f;
    private static float spawnTimer;

    // Set up initial units
    public int numWorkersLeft = 2;
    public int numMeleesLeft = 2;
    public int numRangedLeft = 3;

    // Unit Spawners
    private List<UnitSpawner> unitSpawners;
    public UnitSpawner rangedSpawner;
    public UnitSpawner meleeSpawner;
    public UnitSpawner workerSpawner;
    public void Start()
    {
        spawnTimer = spawnInterval;
        unitSpawners = new List<UnitSpawner>();
        unitSpawners.Add(rangedSpawner);
        unitSpawners.Add(meleeSpawner);
        unitSpawners.Add(workerSpawner);
    }

    // Update is called once per frame
    public override void StateUpdate()
    {
        //foreach (Unit unit in unitsParent.GetComponentInChildren<Unit>())

        // Spawn 2 worker units
        // Spawn 2 melee defense units
        // Spawn 3 ranged defense units
        // Maybe just scrap keeping econ for the ai and just have them send units over to the fish farms to "guard" it
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            Debug.Assert(unitSpawners != null, "unitSpawners is null");
            int randomNum = Random.Range(0, unitSpawners.Count);
            UnitSpawner spawner = unitSpawners[randomNum];
            spawner.RandomSpawn(1, spawner.unitPrefab, spawner.transform.position, spawner.defenceRallyPoints); // Spawns random unit

            spawnTimer = spawnInterval;
        }

    }
}
