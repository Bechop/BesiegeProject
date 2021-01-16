using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class UIObjectif : MonoBehaviour
{
    public TextMeshProUGUI textfield;
    public string phrase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textfield.text = phrase + GameManager.Instance.nbAchievement + "/" + GameManager.Instance.maxAchivement;
    }
}
