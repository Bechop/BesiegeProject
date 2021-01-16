using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour {
    private TextMeshProUGUI tooltipText;

	void Start () {
        tooltipText = GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);
	}

    public void GenerateTooltip(ItemData item)
    {
        string statText = "";
        //if (item.stats.Count > 0)
        //{
        //    foreach(var stat in item.stats)
        //    {
        //        statText += stat.Key.ToString() + ": " + stat.Value + "\n";
        //    }
        //}

        string tooltip = string.Format("<b>{0}</b>\n{1}\n\n<b>{2}</b>", item.title, item.description, statText);
        
        tooltipText.text = tooltip;
        gameObject.SetActive(true);
    }
}
