using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class PlayerController : MonoBehaviour
{
    public enum Option
    {
        MOVE = 0x00001,
        ROTATION = 0x00010,
        ACTION = 0x00100,
        INTERACTION = 0x01000,
        MODIFICATION = 0x10000,
    }

    public Pawn originalPawn;
    [HideInInspector]
    public Pawn currentPawn;

    [HideInInspector]
    public int option;

    // Start is called before the first frame update
    void Awake()
    {
        Utility.SetFlag(ref option, (int)Option.MOVE, true);
        Utility.SetFlag(ref option, (int)Option.ROTATION, true);
        Utility.SetFlag(ref option, (int)Option.ACTION, true);
        Possess(originalPawn);
    }

    // Update is called once per frame
    void Update()
    {
        currentPawn.CustomUpdate();
    }

    public void Possess(Pawn _pawn)
    {
        currentPawn?.UnPossess();
        currentPawn = _pawn;
        currentPawn?.Possess();
    }


}
