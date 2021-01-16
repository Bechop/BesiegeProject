using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingInteractable : MonoBehaviour
{
    public Transform parent;
    public UIKeyField keyFieldPrefab;

    public UIOptionField currentOptionEdit;

    public void OpenDialog()
    {
        // Clean
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }

        // Setup
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        PlayerManager.Instance.SetFlag(PlayerController.Option.MOVE, false);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ROTATION, false);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ACTION, false);
    }

    public void AddKeyField(string _name, KeyCode _keyCode, IKeyBindable _keyBindable)
    {
        GameObject go = Instantiate(keyFieldPrefab.gameObject, parent);
        UIKeyField keyfield = go.GetComponent<UIKeyField>();
        keyfield.keyBindable = _keyBindable;
        keyfield.Init(_name, _keyCode);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialog();
        }
        else if (currentOptionEdit == null && Input.GetKeyDown(KeyCode.F))
        {
            CloseDialog();
        }
    }

    public void CloseDialog()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        PlayerManager.Instance.SetFlag(PlayerController.Option.MOVE, true);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ROTATION, true);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ACTION, true);
    }

    public void UpdateOptionEdit(UIOptionField _uiOptionField)
    {
        currentOptionEdit?.Clear();
        currentOptionEdit = _uiOptionField;
    }
}
