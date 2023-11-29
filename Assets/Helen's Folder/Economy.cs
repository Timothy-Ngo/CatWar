using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Economy : MonoBehaviour
{
    public int workerCatCost = -100;
    public int battleCatCost = -100;
    public int stationCost = -100;
    public int resourceCost = 10;

    public GameObject farmPrefab;

    public int resourceDelay = 2;

    // TODO: restrict buying if player does not have enough money
    public void BuyWorkerCat()
    {
        UI.inst.UpdateCurrencyText(workerCatCost);
    }

    public void BuyBattleCat()
    {
        UI.inst.UpdateCurrencyText(battleCatCost);
    }

   // spawns farm -- shows +money above farm every resourceDelay interval
    public void SpawnFarm()
    {
        Vector3 spawnPosition = new Vector3(0, 55); // hardcoded for now
        GameObject farm = Instantiate(farmPrefab.gameObject, spawnPosition, Quaternion.identity); // weird bug where the prefab wasn't read in as a gameObject --- just manually put .gameObject
        StartResourceCollection(farm.GetComponent<Farm>().cashText);
    }

    public void StartResourceCollection(TextMeshProUGUI resourceMoneyText)
    {
        resourceMoneyText.gameObject.SetActive(false);
        StartCoroutine(CollectResources(resourceMoneyText));
    }

    public void BuyStation()
    {
        UI.inst.UpdateCurrencyText(stationCost);
    }

    // continously collects resources
    IEnumerator CollectResources(TextMeshProUGUI resourceMoneyText)
    {

        yield return new WaitForSeconds(resourceDelay);
        resourceMoneyText.gameObject.SetActive(true);
        UI.inst.UpdateCurrencyText(resourceCost);
        
        UI.inst.ResetTextAlpha(resourceMoneyText);
        UI.inst.FadeTextOut(resourceMoneyText);

        StartCoroutine(CollectResources(resourceMoneyText));
    }
}
