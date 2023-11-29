using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientedPhysics : MonoBehaviour
{
    // Start is called before the first frame update
    void Unit()
    {
        unit = GetComponentInParent<Unit>();
        unit.position = transform.localPosition;
    }

    public Unit unit;


    // Update is called once per frame
    void Update()
    {
        
        if(Utils.ApproximatelyEqual(unit.speed, unit.desiredSpeed)) {
            ;
        } else if(unit.speed < unit.desiredSpeed) {
            unit.speed = unit.speed + unit.unitType.acceleration * Time.deltaTime;
        } else if (unit.speed > unit.desiredSpeed) {
            unit.speed = unit.speed - unit.unitType.acceleration * Time.deltaTime;
        }
        unit.speed = Utils.Clamp(unit.speed, unit.unitType.minSpeed, unit.unitType.maxSpeed);

        //heading
        if (Utils.ApproximatelyEqual(unit.heading, unit.desiredHeading)) {
            ;
        } else if (Utils.AngleDiffPosNeg(unit.desiredHeading, unit.heading) > 0) {
            unit.heading += unit.unitType.turnRate * Time.deltaTime;
        } else if (Utils.AngleDiffPosNeg(unit.desiredHeading, unit.heading) < 0) {
            unit.heading -= unit.unitType.turnRate * Time.deltaTime;
        }
        unit.heading = Utils.Degrees360(unit.heading);
        
        // TODO: Ensure that flipping the x and y with cos and sin is correct
        unit.velocity.y = Mathf.Sin(unit.heading * Mathf.Deg2Rad) * unit.speed; 
        unit.velocity.x = Mathf.Cos(unit.heading * Mathf.Deg2Rad) * unit.speed;

        unit.position = unit.position + unit.velocity * Time.deltaTime;
        transform.localPosition = unit.position;

        eulerRotation.z = unit.heading;
        //transform.localEulerAngles = eulerRotation;
    }

    public Vector3 eulerRotation = Vector3.zero;


}