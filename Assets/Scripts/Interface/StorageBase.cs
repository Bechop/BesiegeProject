using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBase : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public virtual void GiveItem(ItemData itemData, UIItem uiItem = null)
    {
        items.Add(itemData);
    }

    public virtual ItemData CheckForItem(int id)
    {
        return items.Find(item => item.id == id);
    }

    public virtual void RemoveItem(ItemData itemData)
    {
        if (itemData != null)
        {
            items.Remove(itemData);
        }
    }
}
