using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitPawn : Pawn
{
    PlayerController playerController;
    Vehicle vehicle;

    [Header("Movement")]
    public float mouseSensitivity = 150f;
    public float clampAxisX = 45f;
    public float clampAxisY = 80f;
    float xRotation = 0f;
    float yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerController = PlayerManager.Instance.controller;
        vehicle = transform.root.GetComponent<Vehicle>();
    }

    public override void Possess()
    {
        base.Possess();
        vehicle.SetReady();
        vehicle.isUse = true;
    }

    public override void CustomUpdate()
    {
        if (Utility.HasFlag(PlayerManager.Instance.controller.option, (int)PlayerController.Option.ROTATION))
            Rotate();

        if (Input.GetKeyDown(KeyCode.F))
        {
            playerController.Possess(playerController.originalPawn);
        }
    }

    public void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAxisX, clampAxisX);
        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, -clampAxisY, clampAxisY);

        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    public override void UnPossess()
    {
        base.UnPossess();
        vehicle.isUse = false;
    }
}
