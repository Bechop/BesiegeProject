using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class UISaveVehicle : MonoBehaviour
{
    public TMP_InputField inputField;
    public CustomButton button;

    Vehicle vehicle;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialog();
        }
    }

    public void OpendDialog(Vehicle _vehicle)
    {
        vehicle = _vehicle;

        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PlayerManager.Instance.SetFlag(PlayerController.Option.ROTATION, false);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ACTION, false);
    }

    public void CloseDialog()
    {
        inputField.text = "";
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerManager.Instance.SetFlag(PlayerController.Option.ROTATION, true);
        PlayerManager.Instance.SetFlag(PlayerController.Option.ACTION, true);
    }

    public void Save(string osef)
    {
        if (inputField.text == "") return;

        DataManager.Instance.SaveVehicle(vehicle, inputField.text);
        CloseDialog();
    }
}
