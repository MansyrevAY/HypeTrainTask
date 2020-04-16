using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIController : MonoBehaviour, IUIController
{
    [SerializeField] private Text softCurrency;
    [SerializeField] private Text donateCurrency;

    private IServiceLocator ServiceLocator { get; set; }
    private IBonusController BonusController { get; set; }
    private ITimerData timerData;
    
    public void Init(IServiceLocator serviceLocator)
    {
        ServiceLocator = serviceLocator;
        BonusController = serviceLocator.GetService<IBonusController>();
        timerData = (ITimerData)serviceLocator.GetService<IDataController>();
        DontDestroyOnLoad(this);
    }

    public void UpdateCurrency(ICurrencyData data)
    {
        if(softCurrency) softCurrency.text = data.SoftCurrency.ToString();
        if (donateCurrency) donateCurrency.text = data.DonateCurrency.ToString();
    }

    public void IncreaseSoftCurrency(int amount)
    {
        BonusController.GainSoftCurrency(amount);
    }

    public DateTime GetTimer(string name)
    {
        return timerData.GetTimer(name);
    }

    public void SetTimer(string name, DateTime time)
    {
        timerData.SetTimer(name, time);
    }
}
