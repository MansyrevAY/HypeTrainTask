using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using Utility;
using System;

public class DataController : MonoBehaviour, IDataController, ICurrencyData, ISaveManager, ITimerData
{
    #region Static

    public static bool Started;

    #endregion //Static


    private Dictionary<string, DateTime> lockedTimers;
    //public Dictionary<string, DateTime> LockedTimers { get; set; }

    #region Properties

    private long softCurrency;
    [ShowNativeProperty]
    public long SoftCurrency
    {
        get
        {
            return softCurrency;
        }
        set
        {
            softCurrency = value * SoftCurrencyMultiplier;
            uiController.UpdateCurrency(this);
        }
    }
    
    private long donateCurrency;
    [ShowNativeProperty]
    public long DonateCurrency
    {
        get
        {
            return donateCurrency;
        }
        set
        {
            donateCurrency = value;
            uiController.UpdateCurrency(this);
        }
    }

    private int softCurrencyMultiplier;
    [ShowNativeProperty]
    public int SoftCurrencyMultiplier
    {
        get => softCurrencyMultiplier == 0 ? 1 : softCurrencyMultiplier;
        set => softCurrencyMultiplier = value;
    }

    #endregion //Properties

    #region Private

    private IServiceLocator ServiceLocator { get; set; }
    private IUIController uiController { get; set; }
    [ShowNonSerializedField] private int currentScene;

    #endregion //Private

    #region Unity Methods

    private void OnApplicationPause(bool pause)
    {
        if (Started && pause)
        {
            SaveGameState();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameState();
    }
    #endregion //Unity Methods

    public void Init(IServiceLocator serviceLocator)
    {
        ServiceLocator = serviceLocator;
        uiController = ServiceLocator.GetService<IUIController>();
        
        currentScene = 0;

        DontDestroyOnLoad(this);

        Application.targetFrameRate = 100;
        SceneManager.sceneLoaded += OnLevelLoaded;

        TryLoadData();

        LoadGame();

        Started = true;
    }

    public void LoadGame()
    {
        LoadScene(((SceneNames)currentScene).ToString());
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode) { }
    
    #region Scene Management

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    #endregion //Scene Management

    #region Save/Load

    public void SaveGameState()
    {
        SaveData data = new SaveData();

        data.Add("SoftCurrency", SoftCurrency);
        data.Add("DonateCurrency", DonateCurrency);
        data.Add("SoftCurrencyMultiplier", softCurrencyMultiplier);
        data.Add("lockedTimers", lockedTimers);

        if (Serialization.SaveToBinnary<SaveData>(data)) Debug.Log("Game saved succesfuly");
        else Debug.Log("Game not saved");
    }

    private void TryLoadData()
    {
        SaveData data;
        if (!Serialization.LoadFromBinnary<SaveData>(out data)) return;

        SoftCurrency = data.Get<long>("SoftCurrency");
        DonateCurrency = data.Get<long>("DonateCurrency");
        SoftCurrencyMultiplier = data.Get<int>("SoftCurrencyMultiplier");
        lockedTimers = data.Get<Dictionary<string, DateTime>>("lockedTimers");
    }

    public void SetTimer(string name, DateTime dateTime)
    {
        if (lockedTimers == null)
            lockedTimers = new Dictionary<string, DateTime>();
        if(lockedTimers.ContainsKey(name))
        {
            lockedTimers[name] = dateTime;
        }
        else
        {
            lockedTimers.Add(name, dateTime);
        }
    }

    public DateTime GetTimer(string name)
    {
        if (lockedTimers == null)
            lockedTimers = new Dictionary<string, DateTime>();
        if (!lockedTimers.ContainsKey(name))
        {
            Debug.LogWarning("There is no " + name + " timer");
            return DateTime.MaxValue;
        }
        else
        {
            return lockedTimers[name];
        }
    }

    #endregion //Save/Load
}

[System.Serializable]
public class SaveData
{
    private Dictionary<string, object> container;

    public SaveData()
    {
        container = new Dictionary<string, object>();
    }

    public void Add(string name, object obj)
    {
        container.Add(name, obj);
    }

    public T Get<T>(string name)
    {
        try
        {
            return (T)container[name];
        }
        catch
        {
            return default;
        }
    }
}

public enum SceneNames
{
    Game
}
