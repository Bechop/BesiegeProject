using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Cockpit : Block, IInteractable
{
    PlayerManager player;
    CockpitPawn cockpitPawn;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.Instance;
        cockpitPawn = GetComponent<CockpitPawn>();
    }

    public void Interact()
    {
        player.controller.Possess(cockpitPawn);
    }

    public void Focus()
    {
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.INTERACTABLE);
    }

    public void Unfocus()
    {
        MenuManager.Instance.SetCrossair(MenuManager.CrossairStyle.DEFAULT);
    }

}
