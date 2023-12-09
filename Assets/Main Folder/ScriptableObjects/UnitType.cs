using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit data
/// </summary>
[CreateAssetMenu]
public class UnitType : ScriptableObject
{
    [Header("-----POTENTIAL FIELD PARAMETERS-----")]
    public bool isPotentialFieldsMovement = false;
    public float potentialDistanceThreshold = 100;
    [Tooltip("Increase this value to increase attractive strength")] public float attractionCoefficient = 1800;
    public float attractiveExponent = -1;
    [Tooltip("Increase this value to increase the repulsive strength")] public float repulsiveCoefficient = 2000;
    public float repulsiveExponent = -2.0f;
    
    [Header("-----MOVEMENT-----")]
    public float maxSpeed;
    public float minSpeed;
    public float acceleration;
    public float turnRate;

    [Header("-----ATTACK-----")] 
    public bool isRanged;
    public float atkSpeed;
    public float atkDamage;
    
    [Tooltip("What percentage of the collider radius should be the attack range")] public float atkRangeInRad;
    [Tooltip("What percentage of the collider radius should be the detection range")] public float detectionRangeInRad;
    public float health;

    [Header("-----OTHER-----")] 
    public float mass;
    public int cost;
    public string faction;
    public string job;
}
