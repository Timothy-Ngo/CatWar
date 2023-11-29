using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public Units units; 
    public GameObject unitPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spawn(1);
        }
        
        
    }

    public void Spawn(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(unitPrefab, transform.position, Quaternion.identity);
            units.Add(go.GetComponent<Unit>());
            Vector2 newPosition = (Vector2)transform.position + new Vector2(10, 0);
            Unit newUnit = go.GetComponent<Unit>();
            newUnit.position = transform.position;
            DistanceMgr.inst.Initialize();
            Move m = new Move(newUnit, newPosition);
            UnitAI uai = newUnit.GetComponent<UnitAI>();
            Debug.Assert(uai != null);
            uai.AddCommand(m);
        }
    }
}
