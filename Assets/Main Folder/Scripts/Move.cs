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
        foreach (Unit otherUnit in AIMovement.inst.selection.playerUnits.units) {
            if (otherUnit == unit) continue;
            p = DistanceMgr.inst.GetPotential(unit, otherUnit);
            if (p.distance < AIMovement.inst.potentialDistanceThreshold) {
                repulsivePotential += p.direction * unit.unitType.mass *
                                      AIMovement.inst.repulsiveCoefficient * Mathf.Pow(p.diff.magnitude, AIMovement.inst.repulsiveExponent);
            }

        }
        attractivePotential = movePosition - unit.position;
        Vector3 normalizedDiff = attractivePotential.normalized;
        attractivePotential = normalizedDiff * AIMovement.inst.attractionCoefficient *
                              Mathf.Pow(attractivePotential.magnitude, AIMovement.inst.attractiveExponent);
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
        return ((unit.position - movePosition).sqrMagnitude < doneDistanceSq * (AIMovement.inst.selection.playerUnits.units.Count * 0.8f));
    }
    
    
    
}
