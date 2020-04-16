using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

public class TButton : UIBehaviour, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;

    public UIController controller;

    public Text timeLeft;

    public Color normalColor = Color.white;
    public Color highlightedColor = Color.white;
    public Color pressedColor = Color.white;
    public Color disabledColor = Color.white;

    public int disabledTime; // TODO : TIME FOR LOCKDOWN

    public UnityEvent buttonClicked;

    private DateTime buttonWasClicked;
    private bool disabled = false;
    //private int minutesPassed = -1;

    protected override void Start()
    {
        base.Start();

        if (disabledTime == 0)
            return;
        buttonWasClicked = controller.GetTimer(gameObject.name);
        

        if ((DateTime.Now - buttonWasClicked).TotalMinutes > disabledTime)
        {
            int min = Convert.ToInt32((DateTime.Now - buttonWasClicked).TotalMinutes);
            disabled = true;
            timeLeft.text = min >= 1 ? min.ToString() : "";
            targetImage.color = disabledColor;
        }

        StartCoroutine(tickTimer());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (disabled)
            return;

        targetImage.color = pressedColor;
        buttonClicked?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (disabled)
            return;

        targetImage.color = highlightedColor;
        DisableButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disabled)
            return;
        targetImage.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (disabled)
            return;
        targetImage.color = normalColor;
    }

    private void DisableButton()
    {
        if (disabledTime == 0)
            return;

        disabled = true;
        buttonWasClicked = DateTime.Now;
        controller.SetTimer(gameObject.name, buttonWasClicked);
        StartCoroutine(tickTimer());
        targetImage.color = disabledColor;
    }

    private void EnableButton()
    {
        disabled = false;
        timeLeft.text = "";
        targetImage.color = normalColor;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    protected IEnumerator tickTimer()
    {
        while(disabled)
        { 

            if( (DateTime.Now - buttonWasClicked).TotalMinutes >= disabledTime)
            {
                EnableButton();
                StopCoroutine(tickTimer());
            }

            int min = Convert.ToInt32((DateTime.Now - buttonWasClicked).TotalMinutes);
            timeLeft.text = min.ToString();
            yield return new WaitForSeconds(60);
        }
        
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        targetImage.color = normalColor;
    }    
#endif
}
