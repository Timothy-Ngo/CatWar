using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit data
/// </summary>
[CreateAssetMenu]
public class UnitType : ScriptableObject
{
    [Header("-----MOVEMENT-----")]
    public float maxSpeed;

    public float minSpeed;
    public float acceleration;
    public float turnRate;
    
    [Header("-----ATTACK-----")] 
    public float atkSpeed;
    public float atkRange;
    public float atkDamage;
    public float health;

    [Header("-----OTHER-----")] 
    public float mass;
    public float cost;
    public string faction;
    public SpriteRenderer renderer;

}
