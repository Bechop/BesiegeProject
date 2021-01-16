using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : Pawn
{
    [Header("Movement")]
    public float speed = 12f;
    [Space]
    public float mouseSensitivity = 150f;
    float xRotation = 0f;

    [Header("Behaviour")]
    [SerializeField] GameObject dronePrefab = null;

    PlayerController playerController;
    public IInteractable targetInteract = null;

    MenuManager menuManager;

    // Start is called before the first frame update
    void Start()
    {
        playerController = PlayerManager.Instance.controller;
        menuManager = MenuManager.Instance;
    }

    public override void CustomUpdate()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        int layer = 1 << LayerMask.NameToLayer("Anchor");


        if (Utility.HasFlag(PlayerManager.Instance.controller.option, (int)PlayerController.Option.MOVE))
            Move();
        if (Utility.HasFlag(PlayerManager.Instance.controller.option, (int)PlayerController.Option.ROTATION))
            Rotate();

#region ACTIONS
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Vector3 pos = cam.transform.position + cam.transform.forward * 3f;
            Quaternion rot = Quaternion.LookRotation(playerController.transform.forward, Vector3.up);
            GameObject go = Instantiate(dronePrefab, pos, rot);
            playerController.Possess(go.GetComponent<Pawn>());
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (menuManager.inventory.gameObject.activeSelf)
                menuManager.inventory.CloseDialog();
            else
                menuManager.inventory.OpenDialog();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (menuManager.prefabs.gameObject.activeSelf)
                menuManager.prefabs.CloseDialog();
            else
                menuManager.prefabs.OpenDialog();
        }

        // Action contraint
        if (Utility.HasFlag(PlayerManager.Instance.controller.option, (int)PlayerController.Option.ACTION))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (Physics.Raycast(ray, out hit, 5f, ~layer))
                {
                    IInteractable interactable = null;
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Block"))
                    {
                        interactable = hit.transform.GetComponent<IInteractable>();
                    }

                    if (interactable != null)
                    {
                        interactable.Interact();
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
                menuManager.hotBar.selectedItem.item?.Place();

            if (Input.GetMouseButtonDown(1))
            {
                //MenuManager.Instance.hotBar.selectedItem.item?.FirstAction();
            }
        }
#endregion

        // UPDATE UI
        if (Physics.Raycast(ray, out hit, 5f, ~layer))
        {
            IInteractable interactable = null;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                interactable = hit.transform.GetComponent<IInteractable>();
            }
                

            if (targetInteract != interactable)
            {
                targetInteract?.Unfocus();
                targetInteract = interactable;
                targetInteract?.Focus();
            }
        }
        else
        {
            targetInteract?.Unfocus();
            targetInteract = null;
        }
    }

    public void Move()
    {
        // Move
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        move.Normalize();

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }

    public void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camAchor.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}
