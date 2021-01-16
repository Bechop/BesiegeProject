using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    [Header("Base")]
    public int id = 0;
    public string title = "";
    public string description = "";
    public Sprite icon = null;
    public GameObject model = null;

    public virtual void Place(int _index = 0)
    {

    }

    public virtual void OnSelected()
    {
        PlayerManager.Instance.SetObjectInHand(null);
    }
}
