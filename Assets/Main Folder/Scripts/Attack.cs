using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Attack : Command
{
    private Unit targetUnit;
    public Attack(Unit ownUnit, Unit targetUnit) : base(ownUnit)
    {
        this.targetUnit = targetUnit;
    }

    public override void Init()
    {
        // Not used
    }

    public override void Tick()
    {
        // Not used
    }

    public override bool IsDone() // When to stop this command
    {
        return targetUnit == null || !unit.InAttackRange(targetUnit);
    }

    public override void Stop() // What to do when this IsDone()
    {
        // Not used
    }

}
