using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item;
    public Image spriteImage;

    MenuManager menuManager;

    //public OnSlotUpdate onItemAdd;
    //public OnSlotUpdate onItemRemove;

    void Awake()
    {
        UpdateItem(null);
    }

    public void Start()
    {
        menuManager = MenuManager.Instance;
    }

    public void UpdateItem(ItemData _item)
    {
        item = _item;
        if (item != null)
        {
            spriteImage.color = Color.white;
            spriteImage.sprite = item.icon;
        }
        else
        {
            spriteImage.color = Color.clear;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            if (menuManager.grabbedItem.item != null)
            {
                ItemData clone = menuManager.grabbedItem.item;
                menuManager.grabbedItem.UpdateItem(item);
                //onItemRemove.Invoke(this, item);
                UpdateItem(clone);

                //onItemAdd.Invoke(this, item);
            }
            else
            {
                menuManager.grabbedItem.UpdateItem(item);
                //onItemRemove.Invoke(this, item);
                UpdateItem(null);
            }

            menuManager.UpdateTooltip(null);
        }
        else if (menuManager.grabbedItem.item != null)
        {
            UpdateItem(menuManager.grabbedItem.item);
            menuManager.grabbedItem.UpdateItem(null);
            menuManager.UpdateTooltip(item);

            //onItemAdd.Invoke(this, item);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuManager.UpdateTooltip(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menuManager.UpdateTooltip(null);
    }
}
