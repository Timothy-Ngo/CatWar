using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFullDefenseState : AIState
{
    // starts sending spawned cats to defend the nexus
    // any current cats out will go to the defending points to protect the nexus


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

    bool movedExistingUnits = false;

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
        // existing units will move towards the farms --> if they don't run into a worker cat, function call in Unit.cs will move them back on track to the nexus
        if (!movedExistingUnits)
        {
            List<Unit> units = new List<Unit>();
            // get the ai's current units
            foreach (Transform unit in unitsParent.transform)
            {
                units.Add(unit.gameObject.GetComponent<Unit>());
            }



            // move units to defense points to protect the nexus
            foreach (Unit unit in units)
            {
                // get random farm point
                int randomIndex = Random.Range(0, workerSpawner.defenceRallyPoints.Count);
                Vector2 point = workerSpawner.defenceRallyPoints[randomIndex];

                AIMovement.inst.HandlePriorityMove(unit, point);
            }

            movedExistingUnits = true;
        }

        // Spawn 2 worker units
        // Spawn 2 melee defense units
        // Spawn 3 ranged defense units
        // spawned cats will go to defend the nexus
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            Debug.Assert(unitSpawners != null, "unitSpawners is null");
            int randomNum = Random.Range(0, unitSpawners.Count);
            UnitSpawner spawner = unitSpawners[randomNum];
            spawner.RandomSpawn(1, spawner.unitPrefab, spawner.transform.position, spawner.defenceRallyPoints); // Spawns random unit to go to defend the nexus

            spawnTimer = spawnInterval;
        }

    }
}
