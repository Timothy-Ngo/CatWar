using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Unit targetUnit;
    public UnitType unitType;
    public Vector2 position;
    public Vector2 targetPosition;
    public Vector2 velocity;

    public float heading;
    public float desiredHeading;
    
    public float speed; 
    public float desiredSpeed;
    
    public float doneDistanceSq = 1;
    public UnitAI uai;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Vector2 startingPosition, Vector2 tPosition, Unit tUnit, float dmg)
    {
        position = startingPosition;
        targetPosition = tPosition;
        uai = GetComponent<UnitAI>();
        targetUnit = tUnit;
        damage = dmg;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDone())
        {
            targetUnit.TakeDamage(damage);
            Destroy(this.gameObject);
        }
        else
        {
            DHDS dhds = ComputeProjectileDHDS();
            desiredSpeed = dhds.ds;
            desiredHeading = dhds.dh;
        }
    }

    public DHDS ComputeProjectileDHDS()
    {
        Vector2 diff;
        float dhRadians;
        float dhDegrees;
        diff = targetPosition - position;
        dhRadians = Mathf.Atan2(diff.y, diff.x);
        dhDegrees = Utils.Degrees360(Mathf.Rad2Deg * dhRadians);
        return new DHDS(dhDegrees, unitType.maxSpeed);
    }

    public bool IsDone()
    {
        return (position - targetPosition).sqrMagnitude < doneDistanceSq;
    }
}
