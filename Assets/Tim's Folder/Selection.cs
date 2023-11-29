using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public bool isSelecting = false;
    public Vector2 startMousePosition;
    public float selectionSensitivity = 10f;

    public RectTransform selectionBoxPanel;
    public RectTransform uiCanvas;
    private Vector2 wp1;
    private Vector2 wp2;

    public UnitGroup playerUnits;
    public List<Unit> selectedUnits = new List<Unit>();
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { //start box selecting
            isSelecting = true;
            StartBoxSelecting();
        }
        
        if (Input.GetMouseButtonUp(0)) { //end box selecting
            isSelecting = false;
            EndBoxSelecting();
        }
        if (isSelecting)
        {
            UpdateSelectionBox(startMousePosition, Input.mousePosition);
        }

    }
    void StartBoxSelecting()
    {
        selectionBoxPanel.gameObject.SetActive(true);
        startMousePosition = Input.mousePosition;
    }

    void EndBoxSelecting()
    {
        wp1 = Camera.main.ScreenToWorldPoint(startMousePosition);
        wp2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);   
        //Debug.Log((wp2-wp1).sqrMagnitude);
        if ((wp2 - wp1).sqrMagnitude < selectionSensitivity)
        {
            ClearSelection(); // if not small box, then clear selection
        }
        else
        {
            SelectUnitsInBox(startMousePosition, Input.mousePosition);
        }
        selectionBoxPanel.gameObject.SetActive(false);
    }

    public void ClearSelection()
    {
        selectedUnits.Clear();
        foreach (Unit unit in playerUnits.units)
        {
            unit.isSelected = false;
        }
    }
    public void UpdateSelectionBox(Vector2 start, Vector2 end)
    {
        // Update Selection box UI
        selectionBoxPanel.localPosition = new Vector3(start.x - uiCanvas.rect.width/2, start.y - uiCanvas.rect.height/2, 0);
        SetPivotAndAnchors(start, end);
        selectionBoxPanel.sizeDelta = new Vector2(Mathf.Abs(end.x - start.x), Mathf.Abs(start.y - end.y));
    }

    public void SetPivotAndAnchors(Vector2 start, Vector2 end)
    {
        Vector2 diff = end - start;
        // which quadrant?
        if(diff.x >= 0 && diff.y >= 0) {//q1
            SetPAValues(Vector2.zero);
        } else if (diff.x < 0 && diff.y >= 0) { //q2
            SetPAValues(Vector2.right);
        } else if (diff.x < 0 && diff.y < 0) { //q3
            SetPAValues(Vector2.one);
        } else { //q4
            SetPAValues(Vector2.up);
        }
    }
    
    void SetPAValues(Vector2 val)
    {
        selectionBoxPanel.anchorMax = val;
        selectionBoxPanel.anchorMin = val;
        selectionBoxPanel.pivot = val;
    }

    public void SelectUnitsInBox(Vector2 start, Vector2 end)
    {
        // Make sure to check the entities are within your faction
        foreach (Unit unit in playerUnits.units)
        {
            Debug.Log("here");
            Vector2 pos = unit.gameObject.transform.position;
            Debug.Log((wp1.x <= pos.x && pos.x <= wp2.x) && (wp2.y <= pos.y && pos.y <= wp1.y));
            if ((wp1.x <= pos.x && pos.x <= wp2.x) && (wp2.y <= pos.y && pos.y <= wp1.y)
                || (wp2.x <= pos.x && pos.x <= wp1.x) && (wp1.y <= pos.y && pos.y <= wp2.y))
            {
                selectedUnits.Add(unit);
                unit.isSelected = true;
            }
        }
        
    }
}
