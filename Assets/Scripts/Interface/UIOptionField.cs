using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIOptionField : MonoBehaviour
{
    public TextMeshProUGUI m_name;

    public bool isInEdit = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Apply()
    {
        isInEdit = false;
    }

    public virtual void Clear()
    {
        isInEdit = false;
    }
}
