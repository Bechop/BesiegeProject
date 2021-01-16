using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPrefabs : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parent;

    public void AddButton(string _name)
    {
        GameObject go = Instantiate(buttonPrefab, parent);
        go.GetComponent<CustomButton>().onClick.AddListener(() => 
        {
            DataManager.Instance.CreatePrefab(_name);
            CloseDialog();
        });
        go.GetComponentInChildren<TextMeshProUGUI>().text = _name;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerManager.Instance.SetFlag(PlayerController.Option.ROTATION, true);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ACTION, true);
    }
}
