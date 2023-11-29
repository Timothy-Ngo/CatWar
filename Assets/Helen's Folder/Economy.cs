using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour
{
    [SerializeField] int workerCatCost = -100;
    [SerializeField] int battleCatCost = -100;
    [SerializeField] int stationCost = -100;
    [SerializeField] int resourceCost = 10;

    int resourceDelay = 2;

    // TODO: restrict buying if player does not have enough money
    public void BuyWorkerCat()
    {
        UI.inst.UpdateCurrencyText(workerCatCost);
    }

    public void BuyBattleCat()
    {
        UI.inst.UpdateCurrencyText(battleCatCost);
    }

    public void StartResourceCollection()
    {
        StartCoroutine(CollectResources());
    }

    public void BuyStation()
    {
        UI.inst.UpdateCurrencyText(stationCost);
    }

    // continously collects resources
    IEnumerator CollectResources()
    {
        yield return new WaitForSeconds(resourceDelay);
        UI.inst.UpdateCurrencyText(resourceCost);
        StartCoroutine(CollectResources());
    }
}
