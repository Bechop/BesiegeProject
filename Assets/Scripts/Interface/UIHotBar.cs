using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHotBar : MonoBehaviour
{
    [HideInInspector]
    public List<UIItem> uiItems = new List<UIItem>();

    public GameObject slotPrefab;
    public Transform slotPanel;
    public Transform selectedFB;

    public UIItem selectedItem;

    public void Awake()
    {
        for (int i = 0; i < slotPanel.transform.childCount; i++)
        {
            UIItem uiItem = slotPanel.transform.GetChild(i).GetComponent<UIItem>();
            uiItems.Add(uiItem);
        }

        UpdateSelectedItem(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UpdateSelectedItem(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) UpdateSelectedItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) UpdateSelectedItem(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) UpdateSelectedItem(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) UpdateSelectedItem(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) UpdateSelectedItem(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) UpdateSelectedItem(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) UpdateSelectedItem(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9)) UpdateSelectedItem(8);
        else if (Input.GetKeyDown(KeyCode.Alpha0)) UpdateSelectedItem(9);
    }

    public void UpdateSelectedItem(int _index)
    {
        if (_index < 0 || _index >= uiItems.Count || selectedItem == uiItems[_index]) return;

        selectedFB.SetParent(uiItems[_index].transform);
        selectedFB.localPosition = Vector3.zero;

        selectedItem = uiItems[_index];

        selectedItem.item?.OnSelected();
    }

    public void UpdateSlot(int slot, ItemData item)
    {
        uiItems[slot].UpdateItem(item);
    }

    public void AddItem(ItemData item, UIItem uiItem = null)
    {
        if (uiItem)
            UpdateSlot(uiItems.FindIndex(i => i == uiItem), item);
        else
            UpdateSlot(uiItems.FindIndex(i => i.item == null), item);
    }

    public void RemoveItem(ItemData item)
    {
        UpdateSlot(uiItems.FindIndex(i => i.item == item), null);
    }
}
