using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BonusController : MonoBehaviour, IBonusController
{
    private ICurrencyData currencyData;

    public void GainSoftCurrency(int amount)
    {
        currencyData.SoftCurrency += amount;
    }

    public void Init(IServiceLocator serviceLocator)
    {
        DontDestroyOnLoad(this);

        currencyData = (ICurrencyData)serviceLocator.GetService<IDataController>();
    }
}
