using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour {

    [HideInInspector]
    public List<UIItem> uiItems = new List<UIItem>();

    public GameObject slotPrefab;
    public Transform slotPanel;

    public void Awake()
    {
        for (int i = 0; i < slotPanel.transform.childCount; i++)
        {
            UIItem uiItem = slotPanel.transform.GetChild(i).GetComponent<UIItem>();
            uiItems.Add(uiItem);
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialog();
        }
    }

    public void OpenDialog()
    {
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PlayerManager.Instance.SetFlag(PlayerController.Option.ROTATION, false);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ACTION, false);
    }

    public void CloseDialog()
    {
        if (MenuManager.Instance.grabbedItem.item != null)
        {
            MenuManager.Instance.inventory.AddItem(MenuManager.Instance.grabbedItem.item);
        }

        MenuManager.Instance.grabbedItem.UpdateItem(null);
        MenuManager.Instance.UpdateTooltip(null);
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerManager.Instance.SetFlag(PlayerController.Option.ROTATION, true);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ACTION, true);
    }

    public void UpdateSlot(int slot, ItemData item)
    {
        uiItems[slot].UpdateItem(item);
    }

    public void AddItem(ItemData item, UIItem uiItem = null)
    {
        if(uiItem)
            UpdateSlot(uiItems.FindIndex(i => i == uiItem), item);
        else
            UpdateSlot(uiItems.FindIndex(i => i.item == null), item);
    }

    public void RemoveItem(ItemData item)
    {
        UpdateSlot(uiItems.FindIndex(i=> i.item == item), null);
    }
}
