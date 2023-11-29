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

    [SerializeField] TextMeshProUGUI currencyText;
    [SerializeField] int currency = 300;

    private void Start()
    {
        currencyText.text = "Cash: " + currency.ToString();
    }

    public void UpdateCurrencyText(int money)
    {
        currency += money;

        currencyText.text = "Cash: " + currency.ToString();
    }

    bool screenToggled = false;
    public void ToggleBuyScreen(Canvas buyScreen)
    {
        buyScreen.enabled = !screenToggled;
    }
}
