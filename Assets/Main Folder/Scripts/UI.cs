using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI inst;
    private void Awake()
    {
        inst = this;
    }

    public TextMeshProUGUI currencyText;
    public int currency = 300;
    bool screenToggled = false;


    // drag into the inspector
    public TextMeshProUGUI rangedCatBuyButtonText;
    public TextMeshProUGUI meleeCatBuyButtonText;
    public TextMeshProUGUI workerCatBuyButtonText;


    private void Start()
    {
        currencyText.text = "Cash: " + currency.ToString();
    }

    public void UpdateCurrencyText(int money)
    {
        currency += money;

        currencyText.text = "Cash: " + currency.ToString();
    }

    public void ToggleBuyScreen(GameObject buyScreen)
    {
        buyScreen.SetActive(!screenToggled);
        screenToggled = !screenToggled;
    }

    public void FadeTextOut(TextMeshProUGUI fadingText)
    {
        fadingText.CrossFadeAlpha(0f, 1f, false);
    }

    public void ResetTextAlpha(TextMeshProUGUI fadedText)
    {
        fadedText.CrossFadeAlpha(1f, 0f, false);
    }

    public int GetCurrency() 
    { 
        return currency; 
    }
}
