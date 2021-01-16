using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIKeyField : UIOptionField
{
    [Header("Target")]
    public IKeyBindable keyBindable;

    [Header("Data")]
    public KeyCode keycode;
    public CategoryButton button;
    public TextMeshProUGUI keycodeText;

    bool thisFrame = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            return;

        if (thisFrame && isInEdit)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    Debug.Log("apply");
                    keycode = kcode;
                    Apply();
                    break;
                }
            }
        }

        thisFrame = isInEdit;
    }

    public void Init(string _name, KeyCode _keyCode)
    {
        m_name.text = _name;
        keycode = _keyCode;
        keycodeText.text = keycode.ToString();
    }

    public void Edit()
    {
        isInEdit = !isInEdit;
        MenuManager.Instance.settingInteractable.UpdateOptionEdit(this);
    }

    public override void Apply()
    {
        keyBindable?.SetKey(m_name.text, keycode);
        keycodeText.text = keycode.ToString();
        MenuManager.Instance.settingInteractable.UpdateOptionEdit(null);
    }

    public override void Clear()
    {
        button.SetState(CustomButton.State.DEFAULT);

        base.Clear();
    }
}
