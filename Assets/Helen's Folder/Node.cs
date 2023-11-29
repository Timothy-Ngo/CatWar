using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 worldPosition;
    public int gridPositionX;
    public int gridPositionY;

    public Node parentNode = null;

    public float hCost;
    public float gCost;
    public float fCost;

    void Start()
    {
        hCost = 0;
        gCost = float.MaxValue;
        fCost = hCost + gCost;
        parentNode = null;
    }
}
