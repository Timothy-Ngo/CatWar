using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Economy : MonoBehaviour
{
    public static Economy inst;

    private void Awake()
    {
        inst = this;
    }

    public int workerCatCost = -100;
    public int battleCatCost = -100;
    public int stationCost = -100;
    public int resourceCost = 10;

    public GameObject farmPrefab;

    public int resourceDelay = 2;

    public UnitType ironCat;
    public UnitType happy;
    public UnitType catEye;
    public UnitType catWidow;

    // TODO: restrict buying if player does not have enough money
    private void Start()
    {

        UI.inst.rangedCatBuyButtonText.text = "($" + ironCat.cost + ") Spawn Ranged Cat";
        UI.inst.meleeCatBuyButtonText.text = "($" + catWidow.cost + ") Spawn Melee Cat";
        UI.inst.workerCatBuyButtonText.text = "($" + happy.cost + ") Spawn Worker Cat";

    }
    public void BuyCat(UnitType cat)
    {
        if (cat == ironCat)
        {
            battleCatCost = -ironCat.cost;
        }
        else if (cat == happy)
        {
            battleCatCost = -happy.cost;
        }
        else if (cat == catEye)
        {
            battleCatCost = -catEye.cost; 
        }
        else if (cat == catWidow)
        {
            battleCatCost = -catWidow.cost;
        }
        else
        {
            Debug.Log("invalid unit type");
        }

        if (UI.inst.GetCurrency() >= battleCatCost)
        {

            UI.inst.UpdateCurrencyText(battleCatCost);
        }
        else
        {
            Debug.Log("insufficient funds");
        }
    }

   // spawns farm -- shows +money above farm every resourceDelay interval
    public void SpawnFarm()
    {
        Vector3 spawnPosition = new Vector3(0, 55); // hardcoded for now
        GameObject farm = Instantiate(farmPrefab.gameObject, spawnPosition, Quaternion.identity); // weird bug where the prefab wasn't read in as a gameObject --- just manually put .gameObject
        StartResourceCollection(farm.GetComponent<Farm>().cashText);
    }


    public void BuyStation()
    {
        UI.inst.UpdateCurrencyText(stationCost);
    }
    public void StartResourceCollection(TextMeshProUGUI resourceMoneyText)
    {
        resourceMoneyText.gameObject.SetActive(false);
        StartCoroutine(CollectResources(resourceMoneyText));
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


    // TODO: new resource collection method -- move into Unit or UnitAI
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Resources"))
        {
            ;
        }
    }
}
