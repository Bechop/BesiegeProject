using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/CustomButton/CategoryButton")]
public class CategoryButton : CustomButton
{
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
            onClick.Invoke();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (state == State.PRESSED)
            SetState(State.SELECTED);
        else
            SetState(State.HIGHLIGHTED);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (state == State.HIGHLIGHTED)
            SetState(State.DEFAULT);
        else if (state == State.PRESSED)
            SetState(State.DEFAULT);
    }
}
