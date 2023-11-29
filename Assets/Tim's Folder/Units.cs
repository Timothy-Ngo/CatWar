using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Group of unit game object script
/// </summary>
public class Units : MonoBehaviour
{
    public UnitGroup playerUnits;
    // Start is called before the first frame update
    void Start()
    {
        playerUnits.units.Clear();
        foreach (Unit unit in GetComponentsInChildren<Unit>())
        {
            playerUnits.units.Add(unit);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Add(Unit unit)
    {
        playerUnits.units.Add(unit);
        unit.gameObject.transform.parent = transform;
        unit.gameObject.name += " " + playerUnits.units.Count;
    }
}
