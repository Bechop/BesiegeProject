using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/CustomButton/SwitchButton")]
public class SwitchButton : CustomButton
{
    bool isSelected = false;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (state == State.DEFAULT)
            SetState(State.HIGHLIGHTED);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (state == State.HIGHLIGHTED)
        {
            SetState(State.PRESSED);
            isSelected = true;
        }
        else if(state == State.SELECTED)
        {
            SetState(State.PRESSED);
            isSelected = false;
        }

        onClick.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (state == State.PRESSED)
        {
            if(isSelected)
                SetState(State.SELECTED);
            else
                SetState(State.HIGHLIGHTED);
        }
        else
            SetState(State.DEFAULT);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (state == State.HIGHLIGHTED)
            SetState(State.DEFAULT);
        else if (state == State.PRESSED)
            SetState(State.DEFAULT);
    }
}
