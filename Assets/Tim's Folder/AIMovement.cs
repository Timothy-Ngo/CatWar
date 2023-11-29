using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AIMovement : MonoBehaviour
{
    public static AIMovement inst;

    private void Awake()
    {
        inst = this;
    }
    
    [Header("-----POTENTIAL FIELD PARAMETERS-----")]
    public bool isPotentialFieldsMovement = false;
    public float potentialDistanceThreshold = 1000;
    [Tooltip("Increase this value to increase attractive strength")] public float attractionCoefficient = 500;
    public float attractiveExponent = -1;
    [Tooltip("Decrease this value to increase the repulsive strength")] public float repulsiveCoefficient = 60000;
    public float repulsiveExponent = -2.0f;
    
    [Header("-----OTHER-----")]
    public RaycastHit hit;
    public int layerMask;
    public bool astar = false;

    public Selection selection;
    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        if (!astar)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                HandleMove(selection.selectedUnits, wp);
            }
        }
    }

    public void HandleMove(List<Unit> units, Vector2 point)
    {
        foreach (Unit unit in units)
        {
            Move m = new Move(unit, point);
            UnitAI uai = unit.GetComponent<UnitAI>();
            Debug.Assert(uai != null);
            uai.AddCommand(m);
            //uai.SetCommand(m);
        }
    }
}
