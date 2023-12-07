using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePhysics : MonoBehaviour
{
    public Projectile projectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Utils.ApproximatelyEqual(projectile.speed, projectile.desiredSpeed)) {
            ;
        } else if(projectile.speed < projectile.desiredSpeed) {
            projectile.speed = projectile.speed + projectile.unitType.acceleration * Time.deltaTime;
        } else if (projectile.speed > projectile.desiredSpeed) {
            projectile.speed = projectile.speed - projectile.unitType.acceleration * Time.deltaTime;
        }
        projectile.speed = Utils.Clamp(projectile.speed, projectile.unitType.minSpeed, projectile.unitType.maxSpeed);

        //heading
        if (Utils.ApproximatelyEqual(projectile.heading, projectile.desiredHeading)) {
            ;
        } else if (Utils.AngleDiffPosNeg(projectile.desiredHeading, projectile.heading) > 0) {
            projectile.heading += projectile.unitType.turnRate * Time.deltaTime;
        } else if (Utils.AngleDiffPosNeg(projectile.desiredHeading, projectile.heading) < 0) {
            projectile.heading -= projectile.unitType.turnRate * Time.deltaTime;
        }
        projectile.heading = Utils.Degrees360(projectile.heading);
        
        // TODO: Ensure that flipping the x and y with cos and sin is correct
        projectile.velocity.y = Mathf.Sin(projectile.heading * Mathf.Deg2Rad) * projectile.speed; 
        projectile.velocity.x = Mathf.Cos(projectile.heading * Mathf.Deg2Rad) * projectile.speed;

        projectile.position = projectile.position + projectile.velocity * Time.deltaTime;
        transform.localPosition = projectile.position;

        eulerRotation.z = projectile.heading;
        transform.localEulerAngles = eulerRotation;
    }
    
    public Vector3 eulerRotation = Vector3.zero;
    
}
