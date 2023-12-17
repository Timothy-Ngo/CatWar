using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering;

[System.Serializable]
public class Move : Command
{
    
    public Vector2 movePosition;
    
    public Vector3 diff = Vector3.positiveInfinity;
    public float dhRadians;
    public float dhDegrees;
    public float doneDistanceSq = 9;
    public float dh;
    public float angleDiff;
    public float cosValue;
    public float ds;
    
    public Vector2 attractivePotential = Vector3.zero;
    public Vector2 potentialSum = Vector3.zero;
    public Vector2 repulsivePotential = Vector3.zero;
    
    public Move(Unit unit, Vector2 pos) : base(unit)
    {
        movePosition = pos;
    }

    public override void Tick()
    {
        DHDS dhds;
        if (AIMovement.inst.isPotentialFieldsMovement)
        {
            dhds = ComputePotentialDHDS(); 
        }
        else
        {
            dhds = ComputeDHDS(); 
        }
        unit.desiredHeading = dhds.dh;
        unit.desiredSpeed = dhds.ds;
        
    }

    
    public DHDS ComputeDHDS()
    {
        diff = movePosition - unit.position;
        dhRadians = Mathf.Atan2(diff.y, diff.x);
        dhDegrees = Utils.Degrees360(Mathf.Rad2Deg * dhRadians);
        return new DHDS(dhDegrees, unit.unitType.maxSpeed);
    }
    

    public DHDS ComputePotentialDHDS()
    {
        Potential p;
        repulsivePotential = Vector3.zero; // TODO: Determine if we need this to be vector.zero or vector.one  
        foreach (Unit otherUnit in AIMovement.inst.selection.player1Units.units) {
            if (otherUnit == unit) continue;
            Debug.Assert(otherUnit != null, $"{unit.name} is null"); 
            Debug.Assert(unit != null, $"{unit.name} is null");
            p = DistanceMgr.inst.GetPotential(unit, otherUnit);
            if (p.distance < unit.unitType.potentialDistanceThreshold) {
                repulsivePotential += p.direction * unit.unitType.mass *
                                      unit.unitType.repulsiveCoefficient * Mathf.Pow(p.diff.magnitude, unit.unitType.repulsiveExponent);
            }

        }

        foreach (Unit otherUnit in DistanceMgr.inst.obstacles.units)
        {
            if (otherUnit == unit) continue;
            Debug.Assert(unit != null, $"{unit.name} is null");
            Debug.Assert(otherUnit != null, $"{otherUnit.name} is null"); 
            p = DistanceMgr.inst.GetPotential(unit, otherUnit);
            if (p.distance < otherUnit.unitType.potentialDistanceThreshold) {
                repulsivePotential += p.direction * otherUnit.unitType.mass *
                                      otherUnit.unitType.repulsiveCoefficient * Mathf.Pow(p.diff.magnitude, otherUnit.unitType.repulsiveExponent);
            }
        }

        //Debug.Log(repulsivePotential);
        attractivePotential = movePosition - unit.position;
        Vector3 normalizedDiff = attractivePotential.normalized;
        attractivePotential = normalizedDiff * unit.unitType.attractionCoefficient *
                              Mathf.Pow(attractivePotential.magnitude, unit.unitType.attractiveExponent);
        potentialSum = attractivePotential - repulsivePotential;
        
        dh = Utils.Degrees360(Mathf.Rad2Deg * Mathf.Atan2(potentialSum.y, potentialSum.x));
        
        angleDiff = Utils.Degrees360(Utils.AngleDiffPosNeg(dh, unit.heading));
        cosValue = (Mathf.Cos(angleDiff * Mathf.Deg2Rad) + 1) / 2.0f;
        ds = unit.unitType.maxSpeed * cosValue;

        return new DHDS(dh, ds);
    }

    public override void Stop()
    {
        unit.desiredSpeed = 0;
    }

    public override bool IsDone()
    {
        return ((unit.position - movePosition).sqrMagnitude < unit.unitType.doneDistanceSq);
    }
    
    
    
}
