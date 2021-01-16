using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/CustomButton/DefaultButton", 0)]
public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    public enum Style
    {
        SPRITE,
        COLOR
    }

    public enum State
    {
        DEFAULT,
        HIGHLIGHTED,
        PRESSED,
        SELECTED,
        DISABLE,

        NB_STATE
    }

    public Style style = Style.SPRITE;

    
    [Utility.VisibleIf("style", Style.SPRITE)]
    public Sprite m_sDefault;
    [Utility.VisibleIf("style", Style.SPRITE)]
    public Sprite m_sHighlighted;
    [Utility.VisibleIf("style", Style.SPRITE)]
    public Sprite m_sPressed;
    [Utility.VisibleIf("style", Style.SPRITE)]
    public Sprite m_sSelected;
    [Utility.VisibleIf("style", Style.SPRITE)]
    public Sprite m_sDisable;

    [Utility.VisibleIf("style", Style.COLOR)]
    public Color m_cDefault = Color.white;
    [Utility.VisibleIf("style", Style.COLOR)]
    public Color m_cHighlighted = new Color(245,245,245,255);
    [Utility.VisibleIf("style", Style.COLOR)]
    public Color m_cPressed = new Color(200, 200, 200, 255);
    [Utility.VisibleIf("style", Style.COLOR)]
    public Color m_cSelected = new Color(245, 245, 245, 255);
    [Utility.VisibleIf("style", Style.COLOR)]
    public Color m_cDisable = new Color(200, 200, 200, 128);

    public UnityEvent onClick;

    protected Image image;
    public State state = State.DEFAULT;

    protected delegate void OnButtonEvent();
    protected OnButtonEvent[] onButtonEvents;

    public void Awake()
    {
        image = GetComponent<Image>();
        onButtonEvents = new OnButtonEvent[(int)State.NB_STATE];

        for (int i = 0; i < (int)State.NB_STATE; i++)
        {
            switch ((State)i)
            {
                case State.DEFAULT:
                    onButtonEvents[i] += OnDefaultBehaviour;
                    break;
                case State.HIGHLIGHTED:
                    onButtonEvents[i] += OnHighlightedBehaviour;
                    break;
                case State.PRESSED:
                    onButtonEvents[i] += OnPressedBehaviour;
                    break;
                case State.SELECTED:
                    onButtonEvents[i] += OnSelectedBehaviour;
                    break;
                case State.DISABLE:
                    onButtonEvents[i] += OnDisableBehaviour;
                    break;
                default:
                    break;
            }
        }

        SetState(state);
    }

    public virtual void OnDefaultBehaviour()
    {
        switch (style)
        {
            case Style.SPRITE:
                image.sprite = m_sDefault;
                break;
            case Style.COLOR:
                image.color = m_cDefault;
                break;
            default:
                break;
        }

        state = State.DEFAULT;
    }
    public virtual void OnHighlightedBehaviour()
    {
        switch (style)
        {
            case Style.SPRITE:
                image.sprite = m_sHighlighted;
                break;
            case Style.COLOR:
                image.color = m_cHighlighted;
                break;
            default:
                break;
        }
       
        state = State.HIGHLIGHTED;
    }
    public virtual void OnPressedBehaviour()
    {
        switch (style)
        {
            case Style.SPRITE:
                image.sprite = m_sPressed;
                break;
            case Style.COLOR:
                image.color = m_cPressed;
                break;
            default:
                break;
        }

        state = State.PRESSED;
    }
    public virtual void OnSelectedBehaviour()
    {
        switch (style)
        {
            case Style.SPRITE:
                image.sprite = m_sSelected;
                break;
            case Style.COLOR:
                image.color = m_cSelected;
                break;
            default:
                break;
        }

        state = State.SELECTED;
    }
    public virtual void OnDisableBehaviour()
    {
        switch (style)
        {
            case Style.SPRITE:
                image.sprite = m_sDisable;
                break;
            case Style.COLOR:
                image.color = m_cDisable;
                break;
            default:
                break;
        }

        state = State.DISABLE;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (state == State.DEFAULT)
            SetState(State.HIGHLIGHTED);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (state == State.HIGHLIGHTED)
        {
            SetState(State.PRESSED);
            onClick.Invoke();
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if(state == State.PRESSED)
            SetState(State.HIGHLIGHTED);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (state == State.HIGHLIGHTED)
            SetState(State.DEFAULT);
        if (state == State.PRESSED)
            SetState(State.DEFAULT);
    }

    public void SetState(State _state)
    {
        onButtonEvents[(int)_state].Invoke();
    }

    public void Select(bool invoke = false)
    {
        SetState(State.SELECTED);
        if (invoke) onClick.Invoke();
    }

}
