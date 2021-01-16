using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnSlotUpdate(UIItem uiItem, ItemData itemData);

public class MenuManager : Utility.UniqueInstance<MenuManager>
{
    public enum CrossairStyle
    {
        DEFAULT,
        INTERACTABLE,
    }

    [Header("Items")]
    public UIInventory inventory;
    public UIHotBar hotBar;
    public UISettingInteractable settingInteractable;
    public UIPrefabs prefabs;
    public UISaveVehicle save;
    public UIEnd end;
    [Space]
    public UIItem grabbedItem;
    public Tooltip tooltip;

    [Header("Crossair")]
    public Image crossair;
    public List<Sprite> crossairDesigns = new List<Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        inventory.gameObject.SetActive(false);
        settingInteractable.gameObject.SetActive(false);
        prefabs.gameObject.SetActive(false);
        save.gameObject.SetActive(false);
        end.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTooltip(ItemData item)
    {
        if (item != null && grabbedItem.item == null)
            tooltip.GenerateTooltip(item);
        else
            tooltip.gameObject.SetActive(false);
    }

    public void SetCrossair(CrossairStyle _crossairStyle)
    {
        if ((int)_crossairStyle < 0 || (int)_crossairStyle > crossairDesigns.Count)
            return;

        crossair.sprite = crossairDesigns[(int)_crossairStyle];
    }

    public void ResetMenus()
    {
        if (inventory.gameObject.activeSelf) inventory.CloseDialog();
        if (settingInteractable.gameObject.activeSelf) settingInteractable.CloseDialog();
        if (prefabs.gameObject.activeSelf) prefabs.CloseDialog();
        if (save.gameObject.activeSelf) save.CloseDialog();
    }

}
