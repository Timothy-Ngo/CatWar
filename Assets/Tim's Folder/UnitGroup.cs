using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Group of units as a scriptable object
/// </summary>
[CreateAssetMenu]
public class UnitGroup : ScriptableObject
{
    public List<Unit> units = new List<Unit>();

    public void Debug()
    {
        foreach (Unit unit in units)
        {
            UnityEngine.Debug.Log(unit);
        }
    }
}
