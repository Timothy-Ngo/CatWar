using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Unit Gameobject script
/// </summary>
[System.Serializable]
public class Unit : MonoBehaviour
{
    public UnitType unitType;
    private GameObject selectionCircle;

    public bool isSelected
    {
        set { selectionCircle.SetActive(value); }
        get { return selectionCircle.activeSelf; }
    }
    public Vector2 position;
    public Vector2 velocity;

    public float heading;
    public float desiredHeading;
    
    public float speed; 
    public float desiredSpeed;


    public float mass;

    public void Start()
    {
        selectionCircle = transform.GetChild(0).gameObject;
    }

    public void Init(Vector2 startingPosition)
    {
        selectionCircle.SetActive(false);
        isSelected = false;
        position = startingPosition;
        transform.position = position;
    }
}
