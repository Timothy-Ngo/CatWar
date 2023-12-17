using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Potential
{
    public Unit ownship;
    public Unit target;
    public float distance;
    public Vector2 diff;
    public Vector2 relativeVelocity; //Your vel relative to me (yourVel - myVel)
    public Vector2 direction; //normalized diff
    public float relativeBearingDegrees;
    public CPAInfo cpaInfo;
    public float targetAngle;

    public Potential(Unit own, Unit tgt)
    {
        ownship = own;
        target = tgt;
        cpaInfo = new CPAInfo(own, target);

    }
    void InitDefaults()
    {
        distance = 0;
        diff = Vector2.zero;
        relativeVelocity = Vector2.zero;
        direction = Vector2.zero;
        relativeBearingDegrees = 0;
        cpaInfo = new CPAInfo(ownship, target);
        targetAngle = 0;
    }
}


[System.Serializable]
public class CPAInfo
{
    public Unit ownship;
    public Unit target;
    public Vector2 ownShipPosition = Vector2.zero;
    public Vector2 targetPosition = Vector2.zero;
    public float time = 0;
    public float range = 0;
    public float targetRelativeBearing = 0;
    public float targetAbsBearing = 0;
    public float targetAngle;
    public Vector2 relativeVelocity = Vector2.zero;

    Vector2 velDiff = Vector2.zero;
    Vector2 posDiff = Vector2.zero;
    float relSpeedSquared = 0;
    Vector2 diff;

    public CPAInfo(Unit e1, Unit e2)
    {
        ownship = e1;
        target = e2;
    }

    public void ReCompute()
    {
        velDiff = ownship.velocity - target.velocity;
        posDiff = ownship.position - target.position;
        relativeVelocity = target.velocity - ownship.velocity;
        relSpeedSquared = Vector2.Dot(velDiff, velDiff);
        if (relSpeedSquared < Utils.EPSILON * 10)
            time = 0;
        else
            time = -Vector2.Dot(posDiff, velDiff) / relSpeedSquared;
        if (time < 0) time = 0;
        ownShipPosition = ownship.position + ownship.velocity * time;
        targetPosition = target.position + target.velocity * time;
        range = Vector2.Distance(ownShipPosition, targetPosition);

        diff = targetPosition - ownShipPosition;
        targetAbsBearing = Utils.Degrees360(Utils.VectorToHeadingDegrees(diff));
        targetRelativeBearing = Utils.Degrees360(Utils.AngleDiffPosNeg(targetAbsBearing, ownship.heading));
        targetAngle = Utils.Degrees360(targetAbsBearing + 180 - target.heading);

    }
};

public class DistanceMgr : MonoBehaviour
{
    public static DistanceMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public UnitGroup player1Units;
    public UnitGroup player2Units;
    public UnitGroup obstacles;
    public Selection selection;
    
    public Potential[,] potentials2D;
    public Dictionary<Unit, Dictionary<Unit, Potential>> potentialsDictionary;
    //public List<List<Potential>> potentialsList;
    
    public bool isInitialized = false;
    public int i = 0;
    public int j = 0;

    void Start()
    {
    }
    public void Initialize()
    {
        isInitialized = true;
        potentialsDictionary = new Dictionary<Unit, Dictionary<Unit, Potential>>();
        //potentialsList = new List<List<Potential>>();
        List<Unit> totalUnits = player1Units.units.Concat(player2Units.units).Concat(obstacles.units).ToList();
        int n = totalUnits.Count;
        potentials2D = new Potential[n, n];
        i = 0;
        foreach (Unit unit1 in totalUnits) {
            //Debug.Log(unit1.unitType.job);
            Dictionary<Unit, Potential> unit1PotDictionary = new Dictionary<Unit, Potential>();
            List<Potential> unit1PotList = new List<Potential>();
            potentialsDictionary.Add(unit1, unit1PotDictionary);
            //potentialsList.Add(unit1PotList);
            j = 0;
            foreach (Unit unit2 in totalUnits) {
                Potential pot = new Potential(unit1, unit2);
                unit1PotDictionary.Add(unit2, pot);
                unit1PotList.Add(pot);
                potentials2D[i,j] = pot;
                j++;
            }
            i++;
        }
    }
    
    void Stop()
    {
        isInitialized = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I Button pressed");
            Initialize();
        }
        if (isInitialized)
            UpdatePotentials();
        else
            Initialize();
        
    }
    
    void UpdatePotentials()
    {
        Potential p1, p2;
        Unit unit1, unit2;
        List<Unit> totalUnits = player1Units.units.Concat(player2Units.units).Concat(obstacles.units).ToList();
        for(int i = 0; i < totalUnits.Count - 1; i++) {
            /*
            if (unit1 == selection.selectedEntity)
                selectedEntityPotentials = potentialsList[i];
            */
            //don't do diagonal
            for(int j = i+1; j < totalUnits.Count; j++) {

                p1 = potentials2D[i, j];
                p2 = potentials2D[j, i];
                Debug.Assert(p1 != null, "p1 is null");
                Debug.Assert(p2 != null, "p2 is null");
                //p1
                p1.diff = p1.target.position - p1.ownship.position;
                p1.distance = p1.diff.magnitude;
                p1.direction = p1.diff.normalized;
                p1.cpaInfo.ReCompute();
                p1.relativeVelocity = p1.cpaInfo.relativeVelocity;
                p1.targetAngle = p1.cpaInfo.targetAngle;
                p1.relativeBearingDegrees = p1.cpaInfo.targetRelativeBearing;
                //p2
                p2.diff = -p1.diff;
                p2.distance = p1.distance;
                p2.direction = -p1.direction;
                p2.cpaInfo.ReCompute();
                p2.relativeVelocity = p2.cpaInfo.relativeVelocity;
                p2.targetAngle = p2.cpaInfo.targetAngle;
                p2.relativeBearingDegrees = p2.cpaInfo.targetRelativeBearing;
            }
        }
        
    }
    
    public Potential GetPotential(Unit u1, Unit u2)
    {
        Potential p = null;
        // Need to check if u1 or u2 are null
        if (isInitialized)
            p = potentialsDictionary[u1][u2];
        return p;
    }

    /// <summary>
    /// Must be called to update potential field data 
    /// </summary>
    public void RemoveUnit(Unit unit)
    {
        potentialsDictionary.Remove(unit);
        player1Units.units.Remove(unit);
        player2Units.units.Remove(unit);
        
    }
}
