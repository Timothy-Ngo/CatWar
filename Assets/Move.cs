using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering;

public class Move : Command
{
    public Vector2 movePosition;
    
    public Vector3 diff = Vector3.positiveInfinity;
    public float dhRadians;
    public float dhDegrees;
    public float doneDistanceSq = 9;
    public Move(Unit unit, Vector2 pos) : base(unit)
    {
        movePosition = pos;
    }

    public override void Tick()
    {
        DHDS dhds;
        
        dhds = ComputeDHDS(); // Change to ComputePotentialDHDS(); later
        unit.desiredHeading = dhds.dh;
        unit.desiredSpeed = dhds.ds;
    }

    
    public DHDS ComputeDHDS()
    {
        diff = movePosition - unit.position;
        dhRadians = Mathf.Atan2(diff.x, diff.y);
        dhDegrees = Utils.Degrees360(Mathf.Rad2Deg * dhRadians);
        return new DHDS(dhDegrees, unit.unitType.maxSpeed);
    }
    

    public DHDS ComputePotentialDHDS()
    {
        return null;
    }

    public override bool IsDone()
    {
        return ((unit.position - movePosition).sqrMagnitude < doneDistanceSq);
    }
    
}
